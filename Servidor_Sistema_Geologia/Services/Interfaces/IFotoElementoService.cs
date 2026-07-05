using Servidor_Sistema_Geologia.DTO.Gallery;
using Servidor_Sistema_Geologia.Galeria;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IFotoElementoService
{
    Task<FotoElementoDto?> GetByIdAsync(int id);
    Task<IEnumerable<FotoElementoDto>> GetByGaleriaIdAsync(int galeriaId);
    Task<FotoElementoDto> CreateAsync(CreateFotoElementoDto createDto, int galeriaId, int usuarioId);
    Task<FotoElementoDto> UpdateAsync(int id, UpdateFotoElementoDto updateDto, int usuarioId);
    Task DeleteAsync(int id, int usuarioId);
    Task RestoreAsync(int id, int usuarioId);
    Task<byte[]?> GetImagenAsync(int id);
    Task<int?> GetOrCreateGaleriaIdAsync(int elementoId);
    Task<ElementoFotosResponseDto?> GetFotosByElementoAsync(int elementoId, bool isAdmin);
}
