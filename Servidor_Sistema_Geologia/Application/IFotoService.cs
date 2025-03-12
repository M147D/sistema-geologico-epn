using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;

namespace Servidor_Sistema_Geologia.Application
{
	public interface IFotoService<TFoto, TReadFotoDto, TCreateFotoDto>
		where TFoto : FotoElemento
		where TCreateFotoDto : CreateFotoElementoDto
	{
		Task<TReadFotoDto> GetByIdAsync(int id);
		Task<IEnumerable<TReadFotoDto>> GetAllAsync();
		Task<TFoto> CreateAsync(TCreateFotoDto fotoDto, int galeriaId, string userName);
		Task<TFoto> UpdateAsync(int id, TCreateFotoDto dto, int usuarioId);
		Task DeleteAsync(int id, int usuarioId);
	}
}