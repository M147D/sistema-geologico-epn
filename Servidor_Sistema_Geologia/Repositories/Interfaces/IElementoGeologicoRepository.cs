using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.ElementosGeologicos;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IElementoGeologicoRepository
{
    // 🔍 CONSULTAS GENERALES
    Task<ElementoGeologico?> GetByIdAsync(int id);
    Task<ElementoGeologico?> GetByIdWithDetailsAsync(int id); // Include ubicación, galería, fotos
    Task<ElementoGeologico?> GetByCodigoAsync(string codigo);
    Task<PaginatedElementosGeologicosDto> GetAllAsync(ElementoGeologicoFilterDto filter);
    Task<List<ElementoGeologico>> GetAllActiveAsync();
    Task<List<ElementoGeologico>> GetAllAsync(); // Todos incluyendo inactivos
    
    // 🔍 CONSULTAS ESPECÍFICAS POR TIPO
    Task<T?> GetByIdAsync<T>(int id) where T : ElementoGeologico;
    Task<List<T>> GetByTypeAsync<T>(ElementoGeologicoFilterDto? filter = null) where T : ElementoGeologico;
    Task<PaginatedElementosGeologicosDto> GetFosilesAsync(ElementoGeologicoFilterDto filter);
    Task<PaginatedElementosGeologicosDto> GetMineralesAsync(ElementoGeologicoFilterDto filter);
    Task<PaginatedElementosGeologicosDto> GetRocasAsync(ElementoGeologicoFilterDto filter);
    
    // 🔍 CONSULTAS POR UBICACIÓN
    Task<List<ElementoGeologico>> GetByUbicacionAsync(int ubicacionId);
    Task<List<ElementoGeologico>> GetByPaisAsync(int paisId);
    Task<List<ElementoGeologico>> GetByProvinciaAsync(int provinciaId);
    
    // ✏️ OPERACIONES CRUD GENERALES
    Task<ElementoGeologico> CreateAsync(ElementoGeologico elemento);
    Task<ElementoGeologico> UpdateAsync(ElementoGeologico elemento);
    Task<bool> DeleteAsync(int id); // Soft delete
    Task<bool> RestoreAsync(int id); // Restaurar
    
    // ✏️ OPERACIONES CRUD ESPECÍFICAS POR TIPO
    Task<Fosil> CreateFosilAsync(Fosil fosil);
    Task<Mineral> CreateMineralAsync(Mineral mineral);
    Task<Roca> CreateRocaAsync(Roca roca);
    
    Task<Fosil> UpdateFosilAsync(Fosil fosil);
    Task<Mineral> UpdateMineralAsync(Mineral mineral);
    Task<Roca> UpdateRocaAsync(Roca roca);
    
    // 🏢 OPERACIONES CON UBICACIONES
    Task<Ubicacion> CreateUbicacionAsync(Ubicacion ubicacion);
    
    // ✅ VALIDACIONES
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null);
    Task<bool> HasActiveUbicacionAsync(int elementoId);
    Task<bool> HasGaleriaAsync(int elementoId);
    Task<bool> HasFotosAsync(int elementoId);
    
    // 📊 ESTADÍSTICAS
    Task<int> GetTotalCountAsync();
    Task<int> GetActiveCountAsync();
    Task<int> GetInactiveCountAsync();
    Task<Dictionary<string, int>> GetCountByTypeAsync();
    Task<Dictionary<string, int>> GetCountByUbicacionAsync();
    Task<Dictionary<string, int>> GetStatsAsync();
    Task<List<ElementoGeologico>> GetRecentAsync(int count = 10);
    
    // 📈 REPORTES
    Task<Dictionary<string, object>> GetDashboardStatsAsync();
    Task<List<ElementoGeologico>> GetElementosByDateRangeAsync(DateTime desde, DateTime hasta);
    Task<List<ElementoGeologico>> GetElementosByDonanteAsync(string donante);
    
    // 🔄 HISTORIAL DE ACCESO
    Task RegisterAccessAsync(int elementoId, int usuarioId, AccionesUsuario accion);
    Task<List<HistorialAcceso>> GetHistorialAsync(int elementoId);
    Task<HistorialAcceso?> GetLastVisualizacionAsync(int elementoId, int usuarioId);
    Task UpdateOrCreateVisualizacionAsync(int elementoId, int usuarioId);
}
