
namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

/// <summary>
/// Repository interface for Roca entities
/// Inherits from base interface for common operations
/// </summary>
public interface IRocaRepository : IBaseElementoGeologicoRepository<Roca>
{
    // Roca-specific methods
    Task<IEnumerable<Roca>> GetByLitologiaAsync(string litologia);
    Task<IEnumerable<Roca>> GetByTipoRocaAsync(SubtipoRoca tipoRoca);
}