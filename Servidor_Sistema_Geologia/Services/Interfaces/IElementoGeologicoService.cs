using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IElementoGeologicoService
{
    Task<ElementoGeologicoResponseDto> GetByIdWithDetailsAsync(int id, int usuarioId);
    Task<ElementoGeologicoResponseDto> GetByCodigoAsync(string codigo, int usuarioId);
    Task<ElementosGeologicosListResponseDto> GetAllAsync(ElementoGeologicoFilterDto filter);
}
