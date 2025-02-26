using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;

namespace Servidor_Sistema_Geologia.Application
{
	public interface IElementoService<TElemento, TReadDto, TCreateDto>
		where TElemento : ElementoGeologico
		where TReadDto : ElementoGeologicoDto
		where TCreateDto : ElementoGeologicoDto
	{
		Task<TReadDto> GetByIdAsync(int id, int usuarioId);  // Modificado para registrar visualización
		Task<IEnumerable<TReadDto>> GetAllAsync(int usuarioId);  // Modificado para registrar visualización
		Task<TElemento> CreateAsync(TCreateDto elementoDto);  // Sin cambios
		Task<TElemento> CreateElementoConAccesoAsync(TCreateDto dto, int usuarioId);  // Ya existente
		Task<TElemento> UpdateAsync(int id, TCreateDto dto, int usuarioId);  // Modificado para registrar edición
		Task DeleteAsync(int id, int usuarioId);  // Modificado para registrar eliminación
	}
}