using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

/// <summary>
/// Base service interface for ElementoGeologico inheritance hierarchy
/// Provides common business logic for all geological elements
/// </summary>
/// <typeparam name="T">The geological element type (Fosil, Mineral, Roca)</typeparam>
/// <typeparam name="TCreateDto">The create DTO type for the element</typeparam>
/// <typeparam name="TUpdateDto">The update DTO type for the element</typeparam>
public interface IBaseElementoGeologicoService<T, TCreateDto, TUpdateDto> 
    where T : ElementoGeologico
    where TCreateDto : CreateElementoGeologicoBaseDto
    where TUpdateDto : UpdateElementoGeologicoBaseDto
{
    // 🔍 CONSULTAS BASICAS
    Task<ElementoGeologicoResponseDto> GetByIdAsync(int id, int usuarioId);
    Task<PaginatedElementosGeologicosDto> GetAllAsync(ElementoGeologicoFilterDto filter);
    
    // ✏️ OPERACIONES CRUD
    Task<ElementoGeologicoResponseDto> CreateAsync(TCreateDto createDto, int usuarioId);
    Task<ElementoGeologicoResponseDto> UpdateAsync(int id, TUpdateDto updateDto, int usuarioId);
    Task<bool> DeleteAsync(int id, int usuarioId);
    Task<bool> RestoreAsync(int id, int usuarioId);
    
    // ✅ VALIDACIONES
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null);
    
    // 📊 ESTADISTICAS
    Task<Dictionary<string, int>> GetStatsAsync();
    Task<List<T>> GetRecentAsync(int count = 10);
}