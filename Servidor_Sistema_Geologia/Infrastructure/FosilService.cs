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
			var pais = _db.Paises
				.FirstOrDefault(p => p.NombrePais == dto.NombrePais);

			if (pais == null && !string.IsNullOrEmpty(dto.NombrePais))
			{
				pais = new Pais { NombrePais = dto.NombrePais };
				_db.Paises.Add(pais);
				_db.SaveChanges();
			}

			// 2. Buscar o crear la provincia
			var provincia = _db.Provincias
				.FirstOrDefault(p => p.NombreProvincia == dto.NombreProvincia &&
									p.PaisId == pais.Id);

			if (provincia == null && !string.IsNullOrEmpty(dto.NombreProvincia) && pais != null)
			{
				provincia = new Provincia
				{
					NombreProvincia = dto.NombreProvincia,
					PaisId = pais.Id
				};
				_db.Provincias.Add(provincia);
				_db.SaveChanges();
			}

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

			// 4. Crear el estado del elemento
			EstadoElemento estadoElemento;
			if (Enum.TryParse<EstadosElemento>(dto.DescripcionEstado, out var estado))
			{
				estadoElemento = new EstadoElemento { DescripcionEstado = estado };
			}
			else
			{
				estadoElemento = new EstadoElemento { DescripcionEstado = EstadosElemento.Creado };
			}
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
				FechaIngreso = dto.FechaIngreso,
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

			return fosil;
		}

		protected override void UpdateEntity(Fosil fosil, CreateFosilDto dto)
		{
			// Actualizar propiedades básicas
			fosil.Nombre = dto.Nombre;
			fosil.Edad = dto.Edad;
			fosil.Donante = dto.Donante;
			fosil.FechaIngreso = dto.FechaIngreso;
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

			// Actualizar estado si se proporcionó
			if (!string.IsNullOrEmpty(dto.DescripcionEstado))
			{
				ActualizarEstado(fosil, dto);
			}

			_db.SaveChanges();
		}

		private void ActualizarUbicacion(Fosil fosil, CreateFosilDto dto)
		{
			// Lógica para actualizar o crear ubicación
			var pais = _db.Paises.FirstOrDefault(p => p.NombrePais == dto.NombrePais);
			if (pais == null && !string.IsNullOrEmpty(dto.NombrePais))
			{
				pais = new Pais { NombrePais = dto.NombrePais };
				_db.Paises.Add(pais);
				_db.SaveChanges();
			}

			var provincia = _db.Provincias.FirstOrDefault(p =>
				p.NombreProvincia == dto.NombreProvincia && p.PaisId == pais.Id);

			if (provincia == null && !string.IsNullOrEmpty(dto.NombreProvincia) && pais != null)
			{
				provincia = new Provincia
				{
					NombreProvincia = dto.NombreProvincia,
					PaisId = pais.Id
				};
				_db.Provincias.Add(provincia);
				_db.SaveChanges();
			}

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

		private void ActualizarEstado(Fosil fosil, CreateFosilDto dto)
		{
			if (Enum.TryParse<EstadosElemento>(dto.DescripcionEstado, out var estado))
			{
				if (fosil.EstadoElementoId.HasValue)
				{
					var estadoElemento = _db.EstadosElementos.Find(fosil.EstadoElementoId.Value);
					if (estadoElemento != null)
					{
						estadoElemento.DescripcionEstado = estado;
					}
					else
					{
						CrearNuevoEstado(fosil, estado);
					}
				}
				else
				{
					CrearNuevoEstado(fosil, estado);
				}
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