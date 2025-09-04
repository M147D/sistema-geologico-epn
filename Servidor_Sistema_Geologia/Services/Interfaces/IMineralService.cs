using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

/// <summary>
/// Service interface for Mineral entities
/// Inherits from base service interface for common operations
/// </summary>
public interface IMineralService : IBaseElementoGeologicoService<Mineral, CreateMineralDto, UpdateMineralDto>
{
    // Mineral-specific methods
    Task<IEnumerable<Mineral>> GetByLitologiaAsync(string litologia);
    Task<IEnumerable<Mineral>> GetByTipoMineralAsync(SubtipoMineral tipoMineral);
}