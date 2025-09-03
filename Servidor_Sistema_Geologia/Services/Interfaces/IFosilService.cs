using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

/// <summary>
/// Service interface for Fossil entities
/// Inherits from base service interface for common operations
/// </summary>
public interface IFosilService : IBaseElementoGeologicoService<Fosil, CreateFosilDto, UpdateFosilDto>
{
    // Fossil-specific methods
    Task<IEnumerable<Fosil>> GetByPeriodoAsync(string periodo);
    Task<IEnumerable<Fosil>> GetByEspecieAsync(string especie);
    Task<IEnumerable<Fosil>> GetByTipoFosilAsync(SubtipoFosil tipoFosil);
}