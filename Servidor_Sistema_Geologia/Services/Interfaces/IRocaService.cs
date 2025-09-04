using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

/// <summary>
/// Service interface for Roca entities
/// Inherits from base service interface for common operations
/// </summary>
public interface IRocaService : IBaseElementoGeologicoService<Roca, CreateRocaDto, UpdateRocaDto>
{
    // Roca-specific methods
    Task<IEnumerable<Roca>> GetByLitologiaAsync(string litologia);
    Task<IEnumerable<Roca>> GetByTipoRocaAsync(SubtipoRoca tipoRoca);
}