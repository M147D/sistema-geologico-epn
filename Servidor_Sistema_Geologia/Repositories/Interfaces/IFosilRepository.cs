namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

/// <summary>
/// Repository interface for Fosil entities
/// Inherits from base interface for common operations
/// </summary>
public interface IFosilRepository : IBaseElementoGeologicoRepository<Fosil>
{
    // Fossil-specific methods can be added here if needed
    Task<IEnumerable<Fosil>> GetByPeriodoAsync(string periodo);
    Task<IEnumerable<Fosil>> GetByEspecieAsync(string especie);
    Task<IEnumerable<Fosil>> GetByTipoFosilAsync(SubtipoFosil tipoFosil);
}