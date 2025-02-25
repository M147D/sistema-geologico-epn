using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;

namespace Servidor_Sistema_Geologia.Application
{
	public interface IElementoService<TElemento, TReadDto, TCreateDto>
		where TElemento : ElementoGeologico
		where TReadDto : ElementoGeologicoDto
		where TCreateDto : ElementoGeologicoDto
	{
		Task<TReadDto> GetByIdAsync(int id);
		Task<IEnumerable<TReadDto>> GetAllAsync();
		Task<TElemento> CreateAsync(TCreateDto elementoDto);
		Task<TElemento> CreateElementoConAccesoAsync(TCreateDto dto, int usuarioId);
		Task<TElemento> UpdateAsync(int id, TCreateDto dto);
		Task DeleteAsync(int id);
	}
}
