// 2. Ahora actualizamos el servicio base ElementoGeologicoService
using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Constants;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.Application;
using Servidor_Sistema_Geologia.DTO;
using Microsoft.EntityFrameworkCore.Internal;

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
			// Creacion y obtecion del elemento
			var elemento = await CreateAsync(dto);

			// Registrar acceso de creación
			await RegistrarAccesoAsync(usuarioId, elemento.Id, AccionesUsuario.Creacion);

			return elemento;
		}

		// Método para la creación del elemento
		public async Task<TElemento> CreateAsync(TCreateDto dto)
		{
			// Se inicia una transaccion para multiples operaciones en la BD y al finalizar se destruye
			await using var transaction = await _db.Database.BeginTransactionAsync();
			try
			{
				// Se convierte el DTO a la entidad elemento
				var elemento = await ConvertToEntity(dto);

				// Se añade el elemento al contexto de la BD
				_db.Set<TElemento>().Add(elemento);
				await _db.SaveChangesAsync();

				// Se actualiza la información de la galeria
				if (elemento.Galeria != null && elemento.Id > 0)
				{
					elemento.Galeria.ElementoGeologicoId = elemento.Id;
					_db.Entry(elemento.Galeria).State = EntityState.Modified;
					await _db.SaveChangesAsync();
				}

				// Se comprueba la existencia de las fotos en el dto
				try
				{
					dynamic dynamicDto = dto;
					if (dynamicDto.Fotos != null)
					{
						await GuardarFotosAsync(elemento.Id, dynamicDto.Fotos);
					}
				}
				catch (Exception ex)
				{
					throw; // Relanza la excepción para que sea manejada por el bloque try/catch exterior
				}

				// Commit the transaction if all operations succeed
				await transaction.CommitAsync();
				return elemento;
			}
			catch (Exception)
			{
				// Rollback the transaction if any operation fails
				await transaction.RollbackAsync();
				throw;
			}
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

		// Método privado para registrar accesos con contexto independiente y creación previa de objetos
		private async Task RegistrarAccesoAsync(int usuarioId, int? elementoId, AccionesUsuario accion)
		{
			// Prepara los datos antes de interactuar con la base de datos
			DateTime fechaAcceso = DateTime.Now;
			Acceso accesoParaAgregar = null;


			// Si es una acción de visualización, intentamos actualizar un registro existente
			if (accion == AccionesUsuario.Visualizacion && elementoId.HasValue)
			{
				// Buscar si ya existe un registro de visualización para este usuario y elemento
				var accesoExistente = await _db.Accesos
					.FirstOrDefaultAsync(a =>
						a.UsuarioId == usuarioId &&
						a.ElementoGeologicoId == elementoId &&
						a.Accion == AccionesUsuario.Visualizacion);

				if (accesoExistente != null)
				{
					// Actualizar la fecha del acceso existente
					accesoExistente.FechaAcceso = fechaAcceso;
					await _db.SaveChangesAsync();
					return; // Terminamos aquí, ya que solo actualizamos
				}
			}

			// Creamos el objeto acceso fuera de las operaciones de DB
			accesoParaAgregar = new Acceso
			{
				UsuarioId = usuarioId,
				ElementoGeologicoId = elementoId,
				FechaAcceso = fechaAcceso,
				Accion = accion
			};

			// Agregar a la base de datos
			_db.Accesos.Add(accesoParaAgregar);

			await _db.SaveChangesAsync();
		}

		// Métodos de conversión:
		protected virtual TReadDto ConvertToDto(TElemento elemento) => throw new NotImplementedException();
		protected virtual async Task<TElemento> ConvertToEntity(TCreateDto dto) => throw new NotImplementedException();
		protected virtual void UpdateEntity(TElemento elemento, TCreateDto dto) => throw new NotImplementedException();
	}
}