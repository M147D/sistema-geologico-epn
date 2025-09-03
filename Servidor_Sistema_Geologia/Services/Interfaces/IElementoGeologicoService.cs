using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IElementoGeologicoService
{
    // 🔍 CONSULTAS GENERALES
    Task<ElementoGeologicoResponseDto> GetByIdAsync(int id, int usuarioId);
    Task<ElementoGeologicoResponseDto> GetByIdWithDetailsAsync(int id, int usuarioId);
    Task<ElementoGeologicoResponseDto> GetByCodigoAsync(string codigo, int usuarioId);
    Task<ElementosGeologicosListResponseDto> GetAllAsync(ElementoGeologicoFilterDto filter);
    Task<ElementosGeologicosListResponseDto> GetAllActiveAsync();
    
    // 🔍 CONSULTAS ESPECÍFICAS POR TIPO
    Task<ElementosGeologicosListResponseDto> GetFosilesAsync(ElementoGeologicoFilterDto filter);
    Task<ElementosGeologicosListResponseDto> GetMineralesAsync(ElementoGeologicoFilterDto filter);
    Task<ElementosGeologicosListResponseDto> GetRocasAsync(ElementoGeologicoFilterDto filter);
    
    // 🔍 CONSULTAS POR UBICACIÓN
    Task<ElementosGeologicosListResponseDto> GetByUbicacionAsync(int ubicacionId);
    Task<ElementosGeologicosListResponseDto> GetByPaisAsync(int paisId);
    Task<ElementosGeologicosListResponseDto> GetByProvinciaAsync(int provinciaId);
    
    // ✏️ OPERACIONES CRUD - FÓSILES
    Task<ElementoGeologicoResponseDto> CreateFosilAsync(CreateFosilDto createDto);
    Task<ElementoGeologicoResponseDto> GetFosilByIdAsync(int id);
    Task<ElementoGeologicoResponseDto> UpdateFosilAsync(int id, UpdateFosilDto updateDto);
    
    // ✏️ OPERACIONES CRUD - MINERALES
    Task<ElementoGeologicoResponseDto> CreateMineralAsync(CreateMineralDto createDto);
    Task<ElementoGeologicoResponseDto> GetMineralByIdAsync(int id);
    Task<ElementoGeologicoResponseDto> UpdateMineralAsync(int id, UpdateMineralDto updateDto);
    
    // ✏️ OPERACIONES CRUD - ROCAS
    Task<ElementoGeologicoResponseDto> CreateRocaAsync(CreateRocaDto createDto);
    Task<ElementoGeologicoResponseDto> GetRocaByIdAsync(int id);
    Task<ElementoGeologicoResponseDto> UpdateRocaAsync(int id, UpdateRocaDto updateDto);
    
    // ✏️ OPERACIONES COMUNES
    Task<ElementoGeologicoResponseDto> DeleteAsync(int id, int usuarioId); // Soft delete
    Task<ElementoGeologicoResponseDto> DeleteElementoAsync(int id); // Soft delete (for controllers)
    Task<ElementoGeologicoResponseDto> RestoreAsync(int id, int usuarioId); // Restaurar
    
    // ✅ VALIDACIONES
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null);
    Task<bool> CanDeleteAsync(int id); // Verifica si se puede eliminar (sin dependencias críticas)
    
    // 📊 ESTADÍSTICAS Y REPORTES
    Task<ElementoGeologicoResponseDto> GetStatsAsync();
    Task<ElementosGeologicosListResponseDto> GetRecentAsync(int count = 10);
    Task<ElementosGeologicosListResponseDto> GetInactiveAsync(); // Elementos eliminados
    Task<ElementoGeologicoResponseDto> GetDashboardStatsAsync();
    
    // 🔍 BÚSQUEDAS AVANZADAS
    Task<ElementosGeologicosListResponseDto> SearchAsync(string searchTerm, ElementoGeologicoFilterDto? filter = null);
    Task<ElementosGeologicosListResponseDto> GetByDateRangeAsync(DateTime desde, DateTime hasta);
    Task<ElementosGeologicosListResponseDto> GetByDonanteAsync(string donante);
    
    // 🔄 HISTORIAL
    Task<List<HistorialAcceso>> GetHistorialAsync(int elementoId);
    Task RegisterAccessAsync(int elementoId, int usuarioId, AccionesUsuario accion);
}
