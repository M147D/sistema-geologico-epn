// 2. Ahora actualizamos el servicio base ElementoGeologicoService
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

		public async Task<TElemento> UpdateAsync(int id, TCreateDto dto, int usuarioId)
		{
			var elemento = await _db.Set<TElemento>()
				.Include(e => e.Galeria)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (elemento == null)
			{
				throw new KeyNotFoundException("Elemento no encontrado");
			}

			// Actualizar las propiedades del elemento
			UpdateEntity(elemento, dto);

			// Registrar acceso de edición
			await RegistrarAccesoAsync(usuarioId, elemento.Id, AccionesUsuario.Edicion);

			return elemento;
		}

		public async Task<TElemento> CreateElementoConAccesoAsync(TCreateDto dto, int usuarioId)
		{
			var elemento = await CreateAsync(dto);

			// Registrar acceso de creación
			await RegistrarAccesoAsync(usuarioId, elemento.Id, AccionesUsuario.Creacion);

			return elemento;
		}

		public async Task<TElemento> CreateAsync(TCreateDto dto)
		{
			// Este método debe ser implementado por clases derivadas
			var elemento = ConvertToEntity(dto);

			_db.Set<TElemento>().Add(elemento);
			await _db.SaveChangesAsync();

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