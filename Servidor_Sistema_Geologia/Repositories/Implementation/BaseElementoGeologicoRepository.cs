using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

/// <summary>
/// Base repository implementation for ElementoGeologico inheritance hierarchy
/// Handles database connection and mapping for all geological elements
/// </summary>
/// <typeparam name="T">The geological element type (Fosil, Mineral, Roca)</typeparam>
public abstract class BaseElementoGeologicoRepository<T> : IBaseElementoGeologicoRepository<T> 
    where T : ElementoGeologico
{
    protected readonly GestorSistemaGeologia _context;
    protected readonly ILogger<BaseElementoGeologicoRepository<T>> _logger;
    protected abstract string ElementTypeName { get; }
    protected abstract Expression<Func<ElementoGeologico, bool>> TypeFilter { get; }

    protected BaseElementoGeologicoRepository(
        GestorSistemaGeologia context, 
        ILogger<BaseElementoGeologicoRepository<T>> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 🔍 CONSULTAS BASICAS
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _context.ElementosGeologicos
            .OfType<T>()
            .FirstOrDefaultAsync(e => e.Id == id && e.EstadoActivo);
    }

    public virtual async Task<T?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.ElementosGeologicos
            .OfType<T>()
            .Include(e => e.Ubicacion)
                .ThenInclude(u => u!.Pais)
            .Include(e => e.Ubicacion)
                .ThenInclude(u => u!.Provincia)
            .Include(e => e.Galeria)
                .ThenInclude(g => g.Fotos)
            .FirstOrDefaultAsync(e => e.Id == id && e.EstadoActivo);
    }

    public virtual async Task<IEnumerable<T>> GetAllActiveAsync()
    {
        return await _context.ElementosGeologicos
            .OfType<T>()
            .Where(e => e.EstadoActivo)
            .OrderBy(e => e.Nombre)
            .ToListAsync();
    }

    public virtual async Task<PaginatedElementosGeologicosDto> GetAllAsync(ElementoGeologicoFilterDto filter)
    {
        // Build query starting with the typed DbSet
        var query = _context.ElementosGeologicos.OfType<T>().AsQueryable();

        // Apply base filters
        if (!filter.IncludeInactive)
        {
            query = query.Where(e => e.EstadoActivo);
        }

        if (!string.IsNullOrEmpty(filter.Nombre))
        {
            query = query.Where(e => e.Nombre.Contains(filter.Nombre));
        }

        if (!string.IsNullOrEmpty(filter.Codigo))
        {
            query = query.Where(e => e.Codigo.Contains(filter.Codigo));
        }

        if (!string.IsNullOrEmpty(filter.Donante))
        {
            query = query.Where(e => e.Donante.Contains(filter.Donante));
        }

        if (filter.UbicacionId.HasValue)
        {
            query = query.Where(e => e.UbicacionId == filter.UbicacionId.Value);
        }

        if (filter.EstadoActivo.HasValue)
        {
            query = query.Where(e => e.EstadoActivo == filter.EstadoActivo.Value);
        }

        // Apply date filters
        if (filter.FechaIngresoDesde.HasValue)
        {
            query = query.Where(e => e.FechaIngreso >= filter.FechaIngresoDesde.Value);
        }

        if (filter.FechaIngresoHasta.HasValue)
        {
            query = query.Where(e => e.FechaIngreso <= filter.FechaIngresoHasta.Value);
        }

        // Apply custom type-specific filters
        query = ApplyTypeSpecificFilters(query, filter);

        // Apply sorting
        query = ApplySorting(query, filter);

        // Get total count
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        // Get paginated results with full details
        var elementos = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Include(e => e.Ubicacion)
                .ThenInclude(u => u!.Pais)
            .Include(e => e.Ubicacion)
                .ThenInclude(u => u!.Provincia)
            .Include(e => e.Galeria)
                .ThenInclude(g => g.Fotos)
            .ToListAsync();

        // Map to DTOs
        var elementoDtos = elementos.Select(MapToListDto).ToList();

        // Get statistics
        var estadisticas = await GetEstadisticasAsync(filter);

        return new PaginatedElementosGeologicosDto
        {
            ElementosGeologicos = elementoDtos,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            HasPrevious = filter.Page > 1,
            HasNext = filter.Page < totalPages,
            TipoStats = estadisticas.PorTipo,
            EstadoStats = estadisticas.PorEstado
        };
    }

    // ✏️ OPERACIONES CRUD
    public virtual async Task<T> CreateAsync(T elemento)
    {
        elemento.FechaCreacion = DateTime.Now;
        elemento.EstadoActivo = true;

        _context.Set<T>().Add(elemento);
        await _context.SaveChangesAsync();

        _logger.LogInformation("✅ {ElementType} creado: {Nombre} (ID: {Id})", 
            ElementTypeName, elemento.Nombre, elemento.Id);
        
        return elemento;
    }

    public virtual async Task<T> UpdateAsync(T elemento)
    {
        elemento.FechaActualizacion = DateTime.Now;
        
        _context.Set<T>().Update(elemento);
        await _context.SaveChangesAsync();

        _logger.LogInformation("✏️ {ElementType} actualizado: {Nombre} (ID: {Id})", 
            ElementTypeName, elemento.Nombre, elemento.Id);
        
        return elemento;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var elemento = await GetByIdAsync(id);
        if (elemento == null) return false;

        elemento.EstadoActivo = false;
        elemento.FechaActualizacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🗑️ {ElementType} eliminado lógicamente: {Nombre} (ID: {Id})", 
            ElementTypeName, elemento.Nombre, id);
        
        return true;
    }

    public virtual async Task<bool> RestoreAsync(int id)
    {
        var elemento = await _context.Set<T>()
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (elemento == null) return false;

        elemento.EstadoActivo = true;
        elemento.FechaActualizacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🔄 {ElementType} restaurado: {Nombre} (ID: {Id})", 
            ElementTypeName, elemento.Nombre, id);
        
        return true;
    }

    // ✅ VALIDACIONES
    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _context.Set<T>().AnyAsync(e => e.Id == id);
    }

    public virtual async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
    {
        var query = _context.Set<T>().Where(e => e.Codigo == codigo);
        
        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    // 📊 ESTADISTICAS
    public virtual async Task<int> GetTotalCountAsync()
    {
        return await _context.Set<T>().CountAsync();
    }

    public virtual async Task<int> GetActiveCountAsync()
    {
        return await _context.Set<T>().CountAsync(e => e.EstadoActivo);
    }

    public virtual async Task<List<T>> GetRecentAsync(int count = 10)
    {
        return await _context.Set<T>()
            .Where(e => e.EstadoActivo)
            .OrderByDescending(e => e.FechaCreacion)
            .Take(count)
            .ToListAsync();
    }

    // 🎯 METODOS ABSTRACTOS PARA ESPECIALIZACION
    protected abstract IQueryable<T> ApplyTypeSpecificFilters(IQueryable<T> query, ElementoGeologicoFilterDto filter);
    protected abstract IQueryable<T> ApplySorting(IQueryable<T> query, ElementoGeologicoFilterDto filter);
    protected abstract ElementoGeologicoListDto MapToListDto(T elemento);
    protected abstract Task<(Dictionary<string, int> PorTipo, Dictionary<string, int> PorEstado)> GetEstadisticasAsync(ElementoGeologicoFilterDto filter);
}