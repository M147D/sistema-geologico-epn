using AutoMapper;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class MineralService : ElementoGeologicoService<Mineral, MineralDto, CreateMineralDto>
	{
		private readonly IMapper _mapper;

		public MineralService(GestorSistemaGeologia db, IMapper mapper) : base(db)
		{
			_mapper = mapper;
		}

		protected override MineralDto ConvertToDto(Mineral mineral)
		{
			return _mapper.Map<MineralDto>(mineral);
		}

		protected override async Task<Mineral> ConvertToEntity(CreateMineralDto dto)
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
			await _db.SaveChangesAsync();

			// 4. Crear el estado del elemento
			var estadoElemento = new EstadoElemento { DescripcionEstado = EstadosElemento.Creado };
			_db.EstadosElementos.Add(estadoElemento);
			await _db.SaveChangesAsync();

			// 5. Crear galería para el elemento
			var galeria = new GaleriaElementoGeologico
			{
				DetalleGrupo = $"Galería de {dto.Nombre}"
			};
			_db.GaleriaElementosGeologicos.Add(galeria);
			await _db.SaveChangesAsync();

			// 6. Crear el mineral
			var mineral = new Mineral
			{
				TipoMineral = dto.TipoMineral,
				Litologia = dto.Litologia,
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
				GaleriaElementosGeologicoId = galeria.Id,
				Galeria = galeria
			};

			return mineral;
		}

		protected override void UpdateEntity(Mineral mineral, CreateMineralDto dto)
		{
			// Actualizar propiedades básicas
			mineral.Nombre = dto.Nombre;
			mineral.Edad = dto.Edad;
			mineral.Donante = dto.Donante;
			mineral.Codigo = dto.Codigo;
			mineral.Ejemplares = dto.Ejemplares;
			mineral.DocumentosRelacionados = dto.DocumentosRelacionados;
			mineral.LaminaURL = dto.LaminaURL;
			mineral.LaminaExiste = dto.LaminaExiste;

			// Propiedades específicas de Mineral
			mineral.TipoMineral = dto.TipoMineral;
			mineral.Litologia = dto.Litologia;

			// Actualizar ubicación si se proporcionaron datos
			if (!string.IsNullOrEmpty(dto.Latitud) || !string.IsNullOrEmpty(dto.Longitud))
			{
				ActualizarUbicacion(mineral, dto);
			}

			// Cambiar el estado a modificado
			ActualizarEstadoAModificado(mineral);

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

		private void ActualizarUbicacion(Mineral mineral, CreateMineralDto dto)
		{
			// Lógica para actualizar o crear ubicación
			var pais = ObtenerOCrearPais(dto.NombrePais);
			var provincia = ObtenerOCrearProvincia(dto.NombreProvincia, pais?.Id);

			// Verificar si la ubicación existe
			if (mineral.UbicacionId.HasValue)
			{
				var ubicacion = _db.Ubicaciones.Find(mineral.UbicacionId.Value);
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
					CrearNuevaUbicacion(mineral, dto, pais, provincia);
				}
			}
			else
			{
				// Crear nueva ubicación si no existe
				CrearNuevaUbicacion(mineral, dto, pais, provincia);
			}
		}

		private void CrearNuevaUbicacion(Mineral mineral, CreateMineralDto dto, Pais pais, Provincia provincia)
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

			mineral.UbicacionId = nuevaUbicacion.Id;
		}

		private void ActualizarEstadoAModificado(Mineral mineral)
		{
			if (mineral.EstadoElementoId.HasValue)
			{
				var estadoElemento = _db.EstadosElementos.Find(mineral.EstadoElementoId.Value);
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
					CrearNuevoEstado(mineral, EstadosElemento.Modificado);
				}
			}
			else
			{
				CrearNuevoEstado(mineral, EstadosElemento.Modificado);
			}
		}

		private void CrearNuevoEstado(Mineral mineral, EstadosElemento estado)
		{
			var nuevoEstado = new EstadoElemento { DescripcionEstado = estado };
			_db.EstadosElementos.Add(nuevoEstado);
			_db.SaveChanges();

			mineral.EstadoElementoId = nuevoEstado.Id;
		}
	}
}