using Servidor_Sistema_Geologia.Galeria;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IFotoElementoRepository
{
    Task<FotoElemento?> GetByIdAsync(int id);
    Task<IEnumerable<FotoElemento>> GetByGaleriaIdAsync(int galeriaId, bool incluirInactivos = false);
    /// <summary>La foto, su galería y su elemento están activos.</summary>
    Task<bool> EsVisibleAsync(int id);
    /// <summary>La galería y su elemento existen y están activos.</summary>
    Task<bool> GaleriaActivaAsync(int galeriaId);
    Task<FotoElemento> CreateAsync(FotoElemento foto);
    Task<FotoElemento> UpdateAsync(FotoElemento foto);
    Task DeleteAsync(int id, int? usuarioId = null);
    Task RestoreAsync(int id, int? usuarioId = null);
    Task<bool> ExistsAsync(int id);
    Task<int?> GetOrCreateGaleriaIdAsync(int elementoId);
    Task<(int? galeriaId, IEnumerable<FotoElemento> fotos)?> GetGaleriaConFotosAsync(int elementoId, bool soloActivos);
}
