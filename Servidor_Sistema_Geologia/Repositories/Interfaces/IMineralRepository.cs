
namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

/// <summary>
/// Repository interface for Mineral entities
/// Inherits from base interface for common operations
/// </summary>
public interface IMineralRepository : IBaseElementoGeologicoRepository<Mineral>
{
    // Mineral-specific methods
    Task<IEnumerable<Mineral>> GetByLitologiaAsync(string litologia);
    Task<IEnumerable<Mineral>> GetByTipoMineralAsync(SubtipoMineral tipoMineral);
}