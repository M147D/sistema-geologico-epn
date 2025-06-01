using Servidor_Sistema_Geologia.DTO.Ubicaciones;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IPaisRepository
{
    // 🔍 CONSULTAS
    Task<Pais?> GetByIdAsync(int id);
    Task<Pais?> GetByIdWithProvinciasAsync(int id); // Incluye provincias activas
    Task<Pais?> GetByNameAsync(string nombrePais);
    Task<PaginatedPaisesDto> GetAllAsync(PaisFilterDto filter);
    Task<List<Pais>> GetAllActiveAsync();
    Task<List<Pais>> GetAllAsync(); // Todos incluyendo inactivos

    // ✏️ OPERACIONES CRUD
    Task<Pais> CreateAsync(Pais pais);
    Task<Pais> UpdateAsync(Pais pais);
    Task<bool> DeleteAsync(int id); // Soft delete - cambia EstadoActivo = false
    Task<bool> RestoreAsync(int id); // Restaurar - cambia EstadoActivo = true

    // ✅ VALIDACIONES
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string nombrePais, int? excludeId = null);
    Task<bool> HasActiveProvinciasAsync(int paisId);

    // 📊 ESTADÍSTICAS
    Task<int> GetTotalCountAsync();
    Task<int> GetActiveCountAsync();
    Task<int> GetInactiveCountAsync();
    Task<List<Pais>> GetRecentAsync(int count = 10);
    Task<Dictionary<string, int>> GetStatsAsync(); // Estadísticas generales
}