using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Constants;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.Application;
using Servidor_Sistema_Geologia.DTO;
using AutoMapper;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class ElementoGeologicoService<TElemento, TReadDto, TCreateDto> : BaseElementoGeologicoService, IElementoService<TElemento, TReadDto, TCreateDto>
		where TElemento : ElementoGeologico, new()
		where TReadDto : ElementoGeologicoDto, new()
		where TCreateDto : ElementoGeologicoDto, new()
	{
		private readonly IMapper _mapper;

		public ElementoGeologicoService(GestorSistemaGeologia db, IMapper mapper) : base(db)
		{
			_mapper = mapper;
		}

		public async Task<TReadDto> GetByIdAsync(int id)
		{
			var elemento = await _db.Set<TElemento>()
				.Include(e => e.Ubicacion)
					.ThenInclude(u => u.Pais)
				.Include(e => e.Ubicacion)
					.ThenInclude(u => u.Provincia)
				.Include(e => e.EstadoElemento)
				.Include(e => e.Galeria)
					.ThenInclude(g => g.Fotos)
				.FirstOrDefaultAsync(e => e.Id == id && e.EstadoElemento.DescripcionEstado != EstadosElemento.Eliminado);

			if (elemento == null)
			{
				return null;
			}

			return _mapper.Map<TReadDto>(elemento);
		}

		public async Task<IEnumerable<TReadDto>> GetAllAsync()
		{
			var elementos = await _db.Set<TElemento>()
				.Include(e => e.Ubicacion)
					.ThenInclude(u => u.Pais)
				.Include(e => e.Ubicacion)
					.ThenInclude(u => u.Provincia)
				.Include(e => e.EstadoElemento)
				.Include(e => e.Galeria)
					.ThenInclude(g => g.Fotos)
				.Where(e => e.EstadoElemento.DescripcionEstado != EstadosElemento.Eliminado)
				.ToListAsync();

			return _mapper.Map<IEnumerable<TReadDto>>(elementos);
		}

		public async Task<TElemento> UpdateAsync(int id, TCreateDto elementoDto)
		{
			var elemento = await _db.Set<TElemento>()
				.Include(e => e.Galeria)
					.ThenInclude(g => g.Fotos)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (elemento == null)
			{
				throw new KeyNotFoundException($"Elemento con ID {id} no encontrado");
			}

			// Mapear propiedades básicas
			_mapper.Map(elementoDto, elemento);

			// Manejar relaciones especiales
			if (elementoDto.EstadoElemento != null)
			{
				var estado = await ObtenerOcrearEstadoAsync(elementoDto.EstadoElemento);
				elemento.EstadoElementoId = estado.Id;
			}

			if (elementoDto.Ubicacion != null)
			{
				var ubicacion = await ObtenerOcrearUbicacionAsync(elementoDto.Ubicacion);
				elemento.UbicacionId = ubicacion.Id;
			}

			// Actualizar fotos si existen
			if (elementoDto.Fotos != null && elementoDto.Fotos.Any())
			{
				await ActualizarFotosAsync(elemento.Id, elementoDto.Fotos);
			}

			await _db.SaveChangesAsync();
			return elemento;
		}

		public async Task<TElemento> CreateAsync(TCreateDto elementoDto)
		{
			var elemento = _mapper.Map<TElemento>(elementoDto);

			// Gestionar relaciones
			if (elementoDto.Ubicacion != null)
			{
				var ubicacion = await ObtenerOcrearUbicacionAsync(elementoDto.Ubicacion);
				elemento.UbicacionId = ubicacion.Id;
			}

			if (elementoDto.EstadoElemento != null)
			{
				var estado = await ObtenerOcrearEstadoAsync(elementoDto.EstadoElemento);
				elemento.EstadoElementoId = estado.Id;
			}
			else
			{
				// Estado por defecto si no se especifica
				var estadoActivo = await _db.EstadosElementos.FirstOrDefaultAsync(e => e.DescripcionEstado == EstadosElemento.Creado);
				if (estadoActivo == null)
				{
					estadoActivo = new EstadoElemento { DescripcionEstado = EstadosElemento.Creado };
					_db.EstadosElementos.Add(estadoActivo);
					await _db.SaveChangesAsync();
				}
				elemento.EstadoElementoId = estadoActivo.Id;
			}

			_db.Set<TElemento>().Add(elemento);
			await _db.SaveChangesAsync();

			// Crear galería y fotos si existen
			if (elementoDto.Fotos != null && elementoDto.Fotos.Any())
			{
				await GuardarFotosAsync(elemento.Id, elementoDto.Fotos);
			}

			return elemento;
		}

		public async Task<TElemento> CreateElementoConAccesoAsync(TCreateDto dto, int usuarioId)
		{
			// Primero creamos el elemento
			var elemento = await CreateAsync(dto);

			// Luego registramos el acceso
			var acceso = new Acceso
			{
				UsuarioId = usuarioId,
				ElementoGeologicoId = elemento.Id,
				FechaAcceso = DateTime.UtcNow,
				Accion = AccionesUsuario.Creacion
			};

			_db.Accesos.Add(acceso);
			await _db.SaveChangesAsync();

			return elemento;
		}

		public async Task DeleteAsync(int id)
		{
			var elemento = await _db.Set<TElemento>()
				.Include(e => e.EstadoElemento)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (elemento == null)
			{
				throw new KeyNotFoundException($"Elemento con ID {id} no encontrado");
			}

			// Verificar si ya existe un estado "Eliminado"
			var estadoEliminado = await _db.EstadosElementos
				.FirstOrDefaultAsync(e => e.DescripcionEstado == EstadosElemento.Eliminado);

			if (estadoEliminado == null)
			{
				estadoEliminado = new EstadoElemento { DescripcionEstado = EstadosElemento.Eliminado };
				_db.EstadosElementos.Add(estadoEliminado);
				await _db.SaveChangesAsync();
			}

			// Marcar como eliminado (borrado lógico)
			elemento.EstadoElementoId = estadoEliminado.Id;
			await _db.SaveChangesAsync();
		}

		// Método adicional para actualizar fotos existentes
		private async Task ActualizarFotosAsync(int elementoId, IEnumerable<FotoElementoDto> fotosDto)
		{
			// Obtener la galería existente o crear una nueva
			var galeria = await _db.GaleriaElementosGeologicos
				.Include(g => g.Fotos)
				.FirstOrDefaultAsync(g => g.ElementoGeologicoId == elementoId);

			if (galeria == null)
			{
				// Si no existe galería, crear una nueva
				await GuardarFotosAsync(elementoId, fotosDto);
				return;
			}

			// Eliminar fotos existentes (opcional, depende de tu lógica de negocio)
			_db.FotosElementos.RemoveRange(galeria.Fotos);

			// Agregar nuevas fotos
			foreach (var fotoDto in fotosDto)
			{
				var foto = _mapper.Map<FotoElemento>(fotoDto);
				foto.GaleriaElementosGeologicoId = galeria.Id;
				_db.FotosElementos.Add(foto);
			}

			await _db.SaveChangesAsync();
		}
	}
}