using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Constants;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.Application;
using Servidor_Sistema_Geologia.DTO;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class ElementoGeologicoService<TElemento, TReadDto, TCreateDto> : BaseElementoGeologicoService, IElementoService<TElemento, TReadDto, TCreateDto>
		where TElemento : ElementoGeologico, new()
		where TReadDto : ElementoGeologicoDto, new()
		where TCreateDto : ElementoGeologicoDto, new()
	{
		public ElementoGeologicoService(GestorSistemaGeologia db) : base(db) { }

		public async Task<TReadDto> GetByIdAsync(int id, int usuarioId)
		{
			var elemento = await _db.Set<TElemento>()
				.Include(e => e.Galeria)
					.ThenInclude(g => g.Fotos)
				.Include(e => e.Ubicacion)
					.ThenInclude(u => u.Pais)
				.Include(e => e.Ubicacion)
					.ThenInclude(u => u.Provincia)
				.Include(e => e.EstadoElemento)
				.FirstOrDefaultAsync(e => e.Id == id && e.EstadoElemento.DescripcionEstado != EstadosElemento.Eliminado);

			if (elemento == null)
			{
				return null;
			}

			// Registrar acceso de visualización
			await RegistrarAccesoAsync(usuarioId, elemento.Id, AccionesUsuario.Visualizacion);

			return ConvertToDto(elemento);
		}

		public async Task<IEnumerable<TReadDto>> GetAllAsync(int usuarioId)
		{
			var elementos = await _db.Set<TElemento>()
				.Include(e => e.Galeria)
					.ThenInclude(g => g.Fotos)
				.Include(e => e.Ubicacion)
					.ThenInclude(u => u.Pais)
				.Include(e => e.Ubicacion)
					.ThenInclude(u => u.Provincia)
				.Include(e => e.EstadoElemento)
				.Where(e => e.EstadoElemento.DescripcionEstado != EstadosElemento.Eliminado)
				.ToListAsync();

			// Registramos solo un acceso por llamada a GetAll para no llenar la tabla con muchas entradas
			await RegistrarAccesoAsync(usuarioId, null, AccionesUsuario.Visualizacion);

			return elementos.Select(e => ConvertToDto(e));
		}

		public async Task<TElemento> UpdateAsync(int id, TCreateDto elementoDto, int usuarioId)
		{
			var elemento = await _db.Set<TElemento>()
				.Include(e => e.Galeria)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (elemento == null)
			{
				throw new KeyNotFoundException("Elemento no encontrado");
			}

			// Actualizar las propiedades del elemento
			UpdateEntity(elemento, elementoDto);

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

			await _db.SaveChangesAsync();

			// Manejar las fotos si se proporcionan
			if (elementoDto.Fotos != null && elementoDto.Fotos.Any())
			{
				await GuardarFotosAsync(elemento.Id, elementoDto.Fotos);
			}

			// Registrar acceso de edición
			await RegistrarAccesoAsync(usuarioId, elemento.Id, AccionesUsuario.Edicion);

			return elemento;
		}

		public async Task<TElemento> CreateElementoConAccesoAsync(TCreateDto elementoDto, int usuarioId)
		{
			var elemento = await CreateAsync(elementoDto);

			// Registrar acceso de creación
			await RegistrarAccesoAsync(usuarioId, elemento.Id, AccionesUsuario.Creacion);

			return elemento;
		}

		public async Task<TElemento> CreateAsync(TCreateDto elementoDto)
		{
			var ubicacion = elementoDto.Ubicacion != null ? await ObtenerOcrearUbicacionAsync(elementoDto.Ubicacion) : null;
			var estado = elementoDto.EstadoElemento != null ? await ObtenerOcrearEstadoAsync(elementoDto.EstadoElemento) : null;

			var elemento = ConvertToEntity(elementoDto);

			elemento.UbicacionId = ubicacion?.Id;
			elemento.EstadoElementoId = estado?.Id;

			_db.Set<TElemento>().Add(elemento);
			await _db.SaveChangesAsync();

			// Guardar las fotos si existen
			if (elementoDto.Fotos != null && elementoDto.Fotos.Any())
			{
				await GuardarFotosAsync(elemento.Id, elementoDto.Fotos);
			}

			return elemento;
		}

		public async Task DeleteAsync(int id, int usuarioId)
		{
			var elemento = await _db.ElementosGeologicos
				.Include(e => e.EstadoElemento)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (elemento == null)
			{
				throw new KeyNotFoundException("Elemento no encontrado");
			}

			if (elemento.EstadoElemento == null)
			{
				// Si no tiene estado, crear uno
				var estado = new EstadoElemento { DescripcionEstado = EstadosElemento.Eliminado };
				_db.EstadosElementos.Add(estado);
				await _db.SaveChangesAsync();

				elemento.EstadoElementoId = estado.Id;
			}
			else
			{
				elemento.EstadoElemento.DescripcionEstado = EstadosElemento.Eliminado;
			}

			await _db.SaveChangesAsync();

			// Registrar acceso de eliminación
			await RegistrarAccesoAsync(usuarioId, elemento.Id, AccionesUsuario.Eliminacion);
		}

		// Método privado para registrar accesos
		private async Task RegistrarAccesoAsync(int usuarioId, int? elementoId, AccionesUsuario accion)
		{
			var acceso = new Acceso
			{
				UsuarioId = usuarioId,
				ElementoGeologicoId = elementoId,
				FechaAcceso = DateTime.UtcNow,
				Accion = accion
			};

			_db.Accesos.Add(acceso);
			await _db.SaveChangesAsync();
		}

		// Métodos de conversión:
		protected virtual TReadDto ConvertToDto(TElemento elemento) => throw new NotImplementedException();
		protected virtual TElemento ConvertToEntity(TCreateDto dto) => throw new NotImplementedException();
		protected virtual void UpdateEntity(TElemento elemento, TCreateDto dto) => throw new NotImplementedException();
	}
}