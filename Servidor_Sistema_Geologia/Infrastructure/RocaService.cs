// 4. Actualizamos RocaService para usar CreateRocaDto
using AutoMapper;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class RocaService : ElementoGeologicoService<Roca, RocaDto, CreateRocaDto>
	{
		private readonly IMapper _mapper;

		public RocaService(GestorSistemaGeologia db, IMapper mapper) : base(db)
		{
			_mapper = mapper;
		}

		protected override RocaDto ConvertToDto(Roca roca)
		{
			return _mapper.Map<RocaDto>(roca);
		}

		protected override Roca ConvertToEntity(CreateRocaDto dto)
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

			// 6. Crear la roca
			var roca = new Roca
			{
				TipoRoca = dto.TipoRoca,
				Litologia = dto.Litologia,
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

			// 7. Actualizar la relación con la galería después de guardar la roca
			galeria.ElementoGeologicoId = roca.Id;

			return roca;
		}

		protected override void UpdateEntity(Roca roca, CreateRocaDto dto)
		{
			// Actualizar propiedades básicas
			roca.Nombre = dto.Nombre;
			roca.Edad = dto.Edad;
			roca.Donante = dto.Donante;
			roca.FechaIngreso = dto.FechaIngreso;
			roca.Codigo = dto.Codigo;
			roca.Ejemplares = dto.Ejemplares;
			roca.DocumentosRelacionados = dto.DocumentosRelacionados;
			roca.LaminaURL = dto.LaminaURL;
			roca.LaminaExiste = dto.LaminaExiste;

			// Propiedades específicas de Roca
			roca.TipoRoca = dto.TipoRoca;
			roca.Litologia = dto.Litologia;

			// Actualizar ubicación si se proporcionaron datos
			if (!string.IsNullOrEmpty(dto.Latitud) || !string.IsNullOrEmpty(dto.Longitud))
			{
				ActualizarUbicacion(roca, dto);
			}

			// Actualizar estado si se proporcionó
			if (!string.IsNullOrEmpty(dto.DescripcionEstado))
			{
				ActualizarEstado(roca, dto);
			}

			_db.SaveChanges();
		}

		private void ActualizarUbicacion(Roca roca, CreateRocaDto dto)
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
			if (roca.UbicacionId.HasValue)
			{
				var ubicacion = _db.Ubicaciones.Find(roca.UbicacionId.Value);
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
					CrearNuevaUbicacion(roca, dto, pais, provincia);
				}
			}
			else
			{
				// Crear nueva ubicación si no existe
				CrearNuevaUbicacion(roca, dto, pais, provincia);
			}
		}

		private void CrearNuevaUbicacion(Roca roca, CreateRocaDto dto, Pais pais, Provincia provincia)
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

			roca.UbicacionId = nuevaUbicacion.Id;
		}

		private void ActualizarEstado(Roca roca, CreateRocaDto dto)
		{
			if (Enum.TryParse<EstadosElemento>(dto.DescripcionEstado, out var estado))
			{
				if (roca.EstadoElementoId.HasValue)
				{
					var estadoElemento = _db.EstadosElementos.Find(roca.EstadoElementoId.Value);
					if (estadoElemento != null)
					{
						estadoElemento.DescripcionEstado = estado;
					}
					else
					{
						CrearNuevoEstado(roca, estado);
					}
				}
				else
				{
					CrearNuevoEstado(roca, estado);
				}
			}
		}

		private void CrearNuevoEstado(Roca roca, EstadosElemento estado)
		{
			var nuevoEstado = new EstadoElemento { DescripcionEstado = estado };
			_db.EstadosElementos.Add(nuevoEstado);
			_db.SaveChanges();

			roca.EstadoElementoId = nuevoEstado.Id;
		}
	}
}