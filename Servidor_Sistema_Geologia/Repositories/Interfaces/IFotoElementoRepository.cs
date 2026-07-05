using Servidor_Sistema_Geologia.Galeria;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IFotoElementoRepository
{
    Task<FotoElemento?> GetByIdAsync(int id);
    Task<IEnumerable<FotoElemento>> GetByGaleriaIdAsync(int galeriaId);
    Task<FotoElemento> CreateAsync(FotoElemento foto);
    Task<FotoElemento> UpdateAsync(FotoElemento foto);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int?> GetOrCreateGaleriaIdAsync(int elementoId);
    Task<(int? galeriaId, IEnumerable<FotoElemento> fotos)?> GetGaleriaConFotosAsync(int elementoId, bool soloActivos);
}
