using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DTO.Ubicaciones;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class PaisRepository : IPaisRepository
{
    private readonly GestorSistemaGeologia _context;
    private readonly ILogger<PaisRepository> _logger;

    public PaisRepository(GestorSistemaGeologia context, ILogger<PaisRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 🔍 CONSULTAS
    public async Task<Pais?> GetByIdAsync(int id)
    {
        return await _context.Paises
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Pais?> GetByIdWithProvinciasAsync(int id)
    {
        return await _context.Paises
            .Include(p => p.Provincias.Where(pr => pr.EstadoActivo)) // Solo provincias activas
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Pais?> GetByNameAsync(string nombrePais)
    {
        return await _context.Paises
            .FirstOrDefaultAsync(p => p.NombrePais.ToLower() == nombrePais.ToLower());
    }

    public async Task<PaginatedPaisesDto> GetAllAsync(PaisFilterDto filter)
    {
        var query = _context.Paises.AsQueryable();

        // 🔥 POR DEFECTO: Solo mostrar países activos, excepto si se especifica lo contrario
        if (!filter.IncludeInactive)
        {
            query = query.Where(p => p.EstadoActivo);
        }

        // Aplicar filtros
        if (!string.IsNullOrEmpty(filter.NombrePais))
        {
            query = query.Where(p => p.NombrePais.Contains(filter.NombrePais));
        }

        if (filter.EstadoActivo.HasValue)
        {
            query = query.Where(p => p.EstadoActivo == filter.EstadoActivo.Value);
        }

        if (filter.FechaCreacionDesde.HasValue)
        {
            query = query.Where(p => p.FechaCreacion >= filter.FechaCreacionDesde.Value);
        }

        if (filter.FechaCreacionHasta.HasValue)
        {
            query = query.Where(p => p.FechaCreacion <= filter.FechaCreacionHasta.Value);
        }

        // Aplicar ordenamiento
        query = filter.SortBy?.ToLower() switch
        {
            \"nombre\" or \"nombrepais\" => filter.SortDescending ? 
                query.OrderByDescending(p => p.NombrePais) : 
                query.OrderBy(p => p.NombrePais),
            \"fecha\" or \"fechacreacion\" => filter.SortDescending ? 
                query.OrderByDescending(p => p.FechaCreacion) : 
                query.OrderBy(p => p.FechaCreacion),
            \"estado\" or \"estadoactivo\" => filter.SortDescending ? 
                query.OrderByDescending(p => p.EstadoActivo) : 
                query.OrderBy(p => p.EstadoActivo),
            _ => filter.SortDescending ? 
                query.OrderByDescending(p => p.NombrePais) : 
                query.OrderBy(p => p.NombrePais)
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        var paises = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Include(p => p.Provincias.Where(pr => pr.EstadoActivo)) // Solo provincias activas para el conteo
            .Select(p => new PaisListDto
            {
                Id = p.Id,
                NombrePais = p.NombrePais,
                FechaCreacion = p.FechaCreacion,
                EstadoActivo = p.EstadoActivo,
                FechaActualizacion = p.FechaActualizacion,
                TotalProvincias = p.Provincias.Count(pr => pr.EstadoActivo)
            })
            .ToListAsync();

        return new PaginatedPaisesDto
        {
            Paises = paises,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            HasPrevious = filter.Page > 1,
            HasNext = filter.Page < totalPages
        };
    }

    public async Task<List<Pais>> GetAllActiveAsync()
    {
        return await _context.Paises
            .Where(p => p.EstadoActivo)
            .OrderBy(p => p.NombrePais)
            .ToListAsync();
    }

    public async Task<List<Pais>> GetAllAsync()
    {
        return await _context.Paises
            .OrderBy(p => p.NombrePais)
            .ToListAsync();
    }

    // ✏️ OPERACIONES CRUD
    public async Task<Pais> CreateAsync(Pais pais)
    {
        pais.FechaCreacion = DateTime.Now;
        pais.EstadoActivo = true;
        
        _context.Paises.Add(pais);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(\"🌎 País creado: {NombrePais} (ID: {Id})\", pais.NombrePais, pais.Id);
        return pais;
    }

    public async Task<Pais> UpdateAsync(Pais pais)
    {
        pais.FechaActualizacion = DateTime.Now;
        
        _context.Paises.Update(pais);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(\"🌎 País actualizado: {NombrePais} (ID: {Id})\", pais.NombrePais, pais.Id);
        return pais;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pais = await GetByIdAsync(id);
        if (pais == null) return false;

        pais.EstadoActivo = false;
        pais.FechaActualizacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(\"🗑️ País eliminado lógicamente: {NombrePais} (ID: {Id})\", pais.NombrePais, id);
        return true;
    }

    public async Task<bool> RestoreAsync(int id)
    {
        var pais = await GetByIdAsync(id);
        if (pais == null) return false;

        pais.EstadoActivo = true;
        pais.FechaActualizacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(\"🔄 País restaurado: {NombrePais} (ID: {Id})\", pais.NombrePais, id);
        return true;
    }

    // ✅ VALIDACIONES
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Paises.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string nombrePais, int? excludeId = null)
    {
        var query = _context.Paises.Where(p => p.NombrePais.ToLower() == nombrePais.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> HasActiveProvinciasAsync(int paisId)
    {
        return await _context.Provincias
            .AnyAsync(p => p.PaisId == paisId && p.EstadoActivo);
    }

    // 📊 ESTADÍSTICAS
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Paises.CountAsync();
    }

    public async Task<int> GetActiveCountAsync()
    {
        return await _context.Paises.CountAsync(p => p.EstadoActivo);
    }

    public async Task<int> GetInactiveCountAsync()
    {
        return await _context.Paises.CountAsync(p => !p.EstadoActivo);
    }

    public async Task<List<Pais>> GetRecentAsync(int count = 10)
    {
        return await _context.Paises
            .Where(p => p.EstadoActivo)
            .OrderByDescending(p => p.FechaCreacion)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetStatsAsync()
    {
        var stats = new Dictionary<string, int>
        {
            [\"Total\"] = await GetTotalCountAsync(),
            [\"Activos\"] = await GetActiveCountAsync(),
            [\"Inactivos\"] = await GetInactiveCountAsync()
        };

        return stats;
    }
}