using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;

namespace Servidor_Sistema_Geologia.Application
{
	public interface IElementoService<TElemento, TDto>
	where TElemento : ElementoGeologico
	where TDto : ElementoGeologicoDto
	{
		Task<TDto> GetByIdAsync(int id);
		Task<IEnumerable<TDto>> GetAllAsync();
		Task<TElemento> CreateAsync(TDto elementoDto);
		Task<TElemento> CreateElementoConAccesoAsync(TDto dto, int usuarioId);
		Task<TElemento> UpdateAsync(int id, TDto dto);
		Task DeleteAsync(int id);
	}
}
