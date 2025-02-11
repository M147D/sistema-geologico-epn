using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Constants;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.Application;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.DTO.Create;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class ElementoGeologicoService<TElemento, TReadDto, TCreateDto> : BaseElementoGeologicoService, IElementoService<TElemento, TReadDto, TCreateDto>
		where TElemento : ElementoGeologico, new()
		where TReadDto : ElementoGeologicoReadDto, new()
		where TCreateDto : ElementoGeologicoCreateDto, new()
	{
		public ElementoGeologicoService(GestorGeologia db) : base(db) { }

		public async Task<TReadDto> GetByIdAsync(int id)
		{
			var elemento = await _db.Set<TElemento>()
				.Include(e => e.Fotos)
				.Where(c => c.Id == id && c.EstadoElemento.DescripcionEstado != EstadosElemento.Eliminado)
				.FirstOrDefaultAsync();

			if (elemento == null)
			{
				return null;
			}
			return ConvertToDto(elemento);
		}

		public async Task<IEnumerable<TReadDto>> GetAllAsync()
		{
			var elementos = await _db.Set<TElemento>()
				.Include(e => e.Fotos)
				.Include(e => e.Ubicacion)
				.Include(e => e.Ubicacion.Pais)
				.Include(e => e.Ubicacion.Provincia)
				.Include(e => e.EstadoElemento)
				.Where(e => e.EstadoElemento.DescripcionEstado != EstadosElemento.Eliminado)
				.ToListAsync();

			return elementos.Select(e => ConvertToDto(e));
		}

		public async Task<TElemento> UpdateAsync(int id, TCreateDto elementoDto)
		{
			var elemento = await _db.Set<TElemento>()
				.Include(e => e.Fotos)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (elemento == null)
			{
				throw new KeyNotFoundException("Elemento no encontrado");
			}

			elemento.Nombre = elementoDto.Nombre;
			elemento.Edad = elementoDto.Edad;

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

			await GuardarFotosAsync(elemento.Id, elementoDto.Fotos);

			return elemento;
		}

		public async Task<TElemento> CreateElementoConAccesoAsync(TCreateDto elementoDto, int usuarioId)
		{
			var acceso = new Acceso
			{
				UsuarioId = usuarioId,
				FechaAcceso = DateTime.UtcNow,
				Accion = AccionesUsuario.Creacion,
			};

			_db.Accesos.Add(acceso);
			await _db.SaveChangesAsync();

			var elemento = await CreateAsync(elementoDto);

			acceso.ElementoGeologicoId = elemento.Id;
			_db.Entry(acceso).State = EntityState.Modified;
			await _db.SaveChangesAsync();

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

			await GuardarFotosAsync(elemento.Id, elementoDto.Fotos);

			return elemento;
		}

		public async Task DeleteAsync(int id)
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
				throw new InvalidOperationException("El estado del elemento geológico no está definido.");
			}

			elemento.EstadoElemento.DescripcionEstado = EstadosElemento.Eliminado;

			await _db.SaveChangesAsync();
		}

		// Métodos de conversión:
		protected virtual TReadDto ConvertToDto(TElemento elemento) => throw new NotImplementedException();
		protected virtual TElemento ConvertToEntity(TCreateDto dto) => throw new NotImplementedException();
		protected virtual void UpdateEntity(TElemento elemento, TCreateDto dto) => throw new NotImplementedException();
	}
}