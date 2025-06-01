using Servidor_Sistema_Geologia.DTO.Ubicaciones;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IPaisService
{
    // 🔍 CONSULTAS
    Task<PaisResponseDto> GetByIdAsync(int id);
    Task<PaisResponseDto> GetByIdWithProvinciasAsync(int id);
    Task<PaisesListResponseDto> GetAllAsync(PaisFilterDto filter);
    Task<PaisesListResponseDto> GetAllActiveAsync();

    // ✏️ OPERACIONES CRUD
    Task<PaisResponseDto> CreateAsync(CreatePaisDto createDto);
    Task<PaisResponseDto> UpdateAsync(int id, UpdatePaisDto updateDto);
    Task<PaisResponseDto> DeleteAsync(int id); // Soft delete
    Task<PaisResponseDto> RestoreAsync(int id); // Restaurar

    // ✅ VALIDACIONES
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string nombrePais, int? excludeId = null);
    Task<bool> CanDeleteAsync(int id); // Verifica si se puede eliminar (no tiene provincias activas)

    // 📊 ESTADÍSTICAS Y REPORTES
    Task<PaisResponseDto> GetStatsAsync();
    Task<PaisesListResponseDto> GetRecentAsync(int count = 10);
    Task<PaisesListResponseDto> GetInactiveAsync(); // Países eliminados
}