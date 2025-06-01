using Servidor_Sistema_Geologia.DTO.Ubicaciones;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IProvinciaService
{
    // 🔍 CONSULTAS
    Task<ProvinciaResponseDto> GetByIdAsync(int id);
    Task<ProvinciaResponseDto> GetByIdWithPaisAsync(int id);
    Task<ProvinciasListResponseDto> GetAllAsync(ProvinciaFilterDto filter);
    Task<ProvinciasListResponseDto> GetAllActiveAsync();
    Task<ProvinciasListResponseDto> GetByPaisIdAsync(int paisId, bool includeInactive = false);

    // ✏️ OPERACIONES CRUD
    Task<ProvinciaResponseDto> CreateAsync(CreateProvinciaDto createDto);
    Task<ProvinciaResponseDto> UpdateAsync(int id, UpdateProvinciaDto updateDto);
    Task<ProvinciaResponseDto> DeleteAsync(int id); // Soft delete
    Task<ProvinciaResponseDto> RestoreAsync(int id); // Restaurar

    // ✅ VALIDACIONES
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameInPaisAsync(string nombreProvincia, int paisId, int? excludeId = null);
    Task<bool> CanDeleteAsync(int id); // Verifica si se puede eliminar (no tiene ubicaciones activas)

    // 📊 ESTADÍSTICAS Y REPORTES
    Task<ProvinciaResponseDto> GetStatsAsync();
    Task<ProvinciasListResponseDto> GetRecentAsync(int count = 10);
    Task<ProvinciasListResponseDto> GetInactiveAsync(); // Provincias eliminadas
}