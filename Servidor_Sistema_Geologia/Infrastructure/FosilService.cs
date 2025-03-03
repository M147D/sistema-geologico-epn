using AutoMapper;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class FosilService : ElementoGeologicoService<Fosil, FosilDto, CreateFosilDto>
	{
		private readonly IMapper _mapper;

		public FosilService(GestorSistemaGeologia db, IMapper mapper) : base(db)
		{
			_mapper = mapper;
		}

		protected override FosilDto ConvertToDto(Fosil fosil)
		{
			return _mapper.Map<FosilDto>(fosil);
		}

		protected override Fosil ConvertToEntity(CreateFosilDto dto)
		{
			// 1. Buscar o crear el país
			var pais = ObtenerOCrearPais(dto.NombrePais);

			// 2. Buscar o crear la provincia
			var provincia = ObtenerOCrearProvincia(dto.NombreProvincia, pais?.Id);

			// 3. Crear la ubicación
			var ubicacion = new Ubicacion
			{
				Latitud = dto.Latitud,
				Longitud = dto.Longitud,
				Localidad = dto.Localidad,
				Leyenda = dto.Leyenda,
				PaisId = pais?.Id,
				ProvinciaId = provincia?.Id
			};
			_db.Ubicaciones.Add(ubicacion);
			_db.SaveChanges();

			// 4. Crear el estado del elemento (siempre Creado para nuevos elementos)
			var estadoElemento = new EstadoElemento { DescripcionEstado = EstadosElemento.Creado };
			_db.EstadosElementos.Add(estadoElemento);
			_db.SaveChanges();

			// 5. Crear galería para el elemento
			var galeria = new GaleriaElementoGeologico
			{
				DetalleGrupo = $"Galería de {dto.Nombre}"
			};
			_db.GaleriaElementosGeologicos.Add(galeria);
			_db.SaveChanges();

			// 6. Crear el fósil
			var fosil = new Fosil
			{
				Especie = dto.Especie,
				Periodo = dto.Periodo,
				Nombre = dto.Nombre,
				Edad = dto.Edad,
				Donante = dto.Donante,
				FechaIngreso = DateTime.Now,
				Codigo = dto.Codigo,
				Ejemplares = dto.Ejemplares,
				DocumentosRelacionados = dto.DocumentosRelacionados,
				LaminaURL = dto.LaminaURL,
				LaminaExiste = dto.LaminaExiste,
				UbicacionId = ubicacion.Id,
				EstadoElementoId = estadoElemento.Id,
				GaleriaElementosGeologicoId = galeria.Id
			};

			// 7. Actualizar la relación con la galería después de guardar el fósil
			galeria.ElementoGeologicoId = fosil.Id;
			_db.SaveChanges();

			return fosil;
		}

		protected override void UpdateEntity(Fosil fosil, CreateFosilDto dto)
		{
			// Actualizar propiedades básicas
			fosil.Nombre = dto.Nombre;
			fosil.Edad = dto.Edad;
			fosil.Donante = dto.Donante;
			fosil.Codigo = dto.Codigo;
			fosil.Ejemplares = dto.Ejemplares;
			fosil.DocumentosRelacionados = dto.DocumentosRelacionados;
			fosil.LaminaURL = dto.LaminaURL;
			fosil.LaminaExiste = dto.LaminaExiste;

			// Propiedades específicas de Fosil
			fosil.Especie = dto.Especie;
			fosil.Periodo = dto.Periodo;

			// Actualizar ubicación si se proporcionaron datos
			if (!string.IsNullOrEmpty(dto.Latitud) || !string.IsNullOrEmpty(dto.Longitud))
			{
				ActualizarUbicacion(fosil, dto);
			}

			// Cambiar el estado a modificado
			ActualizarEstadoAModificado(fosil);

			_db.SaveChanges();
		}

		private Pais ObtenerOCrearPais(string nombrePais)
		{
			if (string.IsNullOrEmpty(nombrePais))
				return null;

			var pais = _db.Paises.FirstOrDefault(p => p.NombrePais == nombrePais);

			if (pais == null)
			{
				pais = new Pais { NombrePais = nombrePais };
				_db.Paises.Add(pais);
				_db.SaveChanges();
			}

			return pais;
		}

		private Provincia ObtenerOCrearProvincia(string nombreProvincia, int? paisId)
		{
			if (string.IsNullOrEmpty(nombreProvincia) || !paisId.HasValue)
				return null;

			var provincia = _db.Provincias
				.FirstOrDefault(p => p.NombreProvincia == nombreProvincia &&
									 p.PaisId == paisId.Value);

			if (provincia == null)
			{
				provincia = new Provincia
				{
					NombreProvincia = nombreProvincia,
					PaisId = paisId.Value
				};
				_db.Provincias.Add(provincia);
				_db.SaveChanges();
			}

			return provincia;
		}

		private void ActualizarUbicacion(Fosil fosil, CreateFosilDto dto)
		{
			// Lógica para actualizar o crear ubicación
			var pais = ObtenerOCrearPais(dto.NombrePais);
			var provincia = ObtenerOCrearProvincia(dto.NombreProvincia, pais?.Id);

			// Verificar si la ubicación existe
			if (fosil.UbicacionId.HasValue)
			{
				var ubicacion = _db.Ubicaciones.Find(fosil.UbicacionId.Value);
				if (ubicacion != null)
				{
					// Actualizar ubicación existente
					ubicacion.Latitud = dto.Latitud ?? ubicacion.Latitud;
					ubicacion.Longitud = dto.Longitud ?? ubicacion.Longitud;
					ubicacion.Localidad = dto.Localidad ?? ubicacion.Localidad;
					ubicacion.Leyenda = dto.Leyenda ?? ubicacion.Leyenda;
					ubicacion.PaisId = pais?.Id ?? ubicacion.PaisId;
					ubicacion.ProvinciaId = provincia?.Id ?? ubicacion.ProvinciaId;
				}
				else
				{
					// Crear nueva ubicación si no se encuentra la existente
					CrearNuevaUbicacion(fosil, dto, pais, provincia);
				}
			}
			else
			{
				// Crear nueva ubicación si no existe
				CrearNuevaUbicacion(fosil, dto, pais, provincia);
			}
		}

		private void CrearNuevaUbicacion(Fosil fosil, CreateFosilDto dto, Pais pais, Provincia provincia)
		{
			var nuevaUbicacion = new Ubicacion
			{
				Latitud = dto.Latitud,
				Longitud = dto.Longitud,
				Localidad = dto.Localidad,
				Leyenda = dto.Leyenda,
				PaisId = pais?.Id,
				ProvinciaId = provincia?.Id
			};

			_db.Ubicaciones.Add(nuevaUbicacion);
			_db.SaveChanges();

			fosil.UbicacionId = nuevaUbicacion.Id;
		}

		private void ActualizarEstadoAModificado(Fosil fosil)
		{
			if (fosil.EstadoElementoId.HasValue)
			{
				var estadoElemento = _db.EstadosElementos.Find(fosil.EstadoElementoId.Value);
				if (estadoElemento != null)
				{
					// Solo actualizar si no está eliminado
					if (estadoElemento.DescripcionEstado != EstadosElemento.Eliminado)
					{
						estadoElemento.DescripcionEstado = EstadosElemento.Modificado;
					}
				}
				else
				{
					CrearNuevoEstado(fosil, EstadosElemento.Modificado);
				}
			}
			else
			{
				CrearNuevoEstado(fosil, EstadosElemento.Modificado);
			}
		}

		private void CrearNuevoEstado(Fosil fosil, EstadosElemento estado)
		{
			var nuevoEstado = new EstadoElemento { DescripcionEstado = estado };
			_db.EstadosElementos.Add(nuevoEstado);
			_db.SaveChanges();

			fosil.EstadoElementoId = nuevoEstado.Id;
		}
	}
}