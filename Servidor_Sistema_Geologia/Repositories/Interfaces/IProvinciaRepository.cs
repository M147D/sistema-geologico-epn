using Servidor_Sistema_Geologia.DTO.Ubicaciones;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IProvinciaRepository
{
    // 🔍 CONSULTAS
    Task<Provincia?> GetByIdAsync(int id);
    Task<Provincia?> GetByIdWithPaisAsync(int id); // Incluye información del país
    Task<Provincia?> GetByNameAsync(string nombreProvincia, int paisId);
    Task<PaginatedProvinciasDto> GetAllAsync(ProvinciaFilterDto filter);
    Task<List<Provincia>> GetAllActiveAsync();
    Task<List<Provincia>> GetByPaisIdAsync(int paisId, bool includeInactive = false);
    Task<List<Provincia>> GetAllAsync(); // Todos incluyendo inactivos

    // ✏️ OPERACIONES CRUD
    Task<Provincia> CreateAsync(Provincia provincia);
    Task<Provincia> UpdateAsync(Provincia provincia);
    Task<bool> DeleteAsync(int id); // Soft delete - cambia EstadoActivo = false
    Task<bool> RestoreAsync(int id); // Restaurar - cambia EstadoActivo = true

    // ✅ VALIDACIONES
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameInPaisAsync(string nombreProvincia, int paisId, int? excludeId = null);
    Task<bool> HasActiveUbicacionesAsync(int provinciaId);

    // 📊 ESTADÍSTICAS
    Task<int> GetTotalCountAsync();
    Task<int> GetActiveCountAsync();
    Task<int> GetInactiveCountAsync();
    Task<int> GetCountByPaisAsync(int paisId);
    Task<List<Provincia>> GetRecentAsync(int count = 10);
    Task<Dictionary<string, int>> GetStatsAsync(); // Estadísticas generales
}