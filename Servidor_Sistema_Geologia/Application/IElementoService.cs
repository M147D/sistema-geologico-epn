using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;

namespace Servidor_Sistema_Geologia.Application
{
	public interface IElementoService<TElemento, TReadDto, TCreateDto>
		where TElemento : ElementoGeologico
		where TReadDto : ElementoGeologicoDto
	{
		Task<TReadDto> GetByIdAsync(int id, int usuarioId);
		Task<IEnumerable<TReadDto>> GetAllAsync();
		Task<TElemento> CreateAsync(TCreateDto elementoDto);
		Task<TElemento> CreateElementoConAccesoAsync(TCreateDto dto, int usuarioId);
		Task<TElemento> UpdateAsync(int id, TCreateDto dto, int usuarioId);
		Task DeleteAsync(int id, int usuarioId);
		Task<IEnumerable<TReadDto>> FilterAsync(FiltroElementoDto filtro);
	}
}