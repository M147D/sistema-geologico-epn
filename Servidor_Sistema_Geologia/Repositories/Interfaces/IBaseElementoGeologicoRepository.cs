using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

/// <summary>
/// Base repository interface for ElementoGeologico inheritance hierarchy
/// Provides common CRUD operations for all geological elements
/// </summary>
/// <typeparam name="T">The geological element type (Fosil, Mineral, Roca)</typeparam>
public interface IBaseElementoGeologicoRepository<T> where T : ElementoGeologico
{
    // 🔍 CONSULTAS BASICAS
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<T>> GetAllActiveAsync();
    Task<PaginatedElementosGeologicosDto> GetAllAsync(ElementoGeologicoFilterDto filter);
    
    // ✏️ OPERACIONES CRUD
    Task<T> CreateAsync(T elemento);
    Task<T> UpdateAsync(T elemento);
    Task<bool> DeleteAsync(int id);
    Task<bool> RestoreAsync(int id);
    
    // ✅ VALIDACIONES
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null);
    
    // 📊 ESTADISTICAS
    Task<int> GetTotalCountAsync();
    Task<int> GetActiveCountAsync();
    Task<List<T>> GetRecentAsync(int count = 10);
}