using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DTO.Ubicaciones;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class ProvinciaRepository : IProvinciaRepository
{
    private readonly GestorSistemaGeologia _context;
    private readonly ILogger<ProvinciaRepository> _logger;

    public ProvinciaRepository(GestorSistemaGeologia context, ILogger<ProvinciaRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 🔍 CONSULTAS
    public async Task<Provincia?> GetByIdAsync(int id)
    {
        return await _context.Provincias
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Provincia?> GetByIdWithPaisAsync(int id)
    {
        return await _context.Provincias
            .Include(p => p.Pais)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Provincia?> GetByNameAsync(string nombreProvincia, int paisId)
    {
        return await _context.Provincias
            .FirstOrDefaultAsync(p => p.NombreProvincia.ToLower() == nombreProvincia.ToLower() 
                                    && p.PaisId == paisId);
    }

    public async Task<PaginatedProvinciasDto> GetAllAsync(ProvinciaFilterDto filter)
    {
        var query = _context.Provincias.AsQueryable();

        // 🔥 POR DEFECTO: Solo mostrar provincias activas, excepto si se especifica lo contrario
        if (!filter.IncludeInactive)
        {
            query = query.Where(p => p.EstadoActivo);
        }

        // Aplicar filtros
        if (!string.IsNullOrEmpty(filter.NombreProvincia))
        {
            query = query.Where(p => p.NombreProvincia.Contains(filter.NombreProvincia));
        }

        if (filter.PaisId.HasValue)
        {
            query = query.Where(p => p.PaisId == filter.PaisId.Value);
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
            "nombre" or "nombreprovincia" => filter.SortDescending ? 
                query.OrderByDescending(p => p.NombreProvincia) : 
                query.OrderBy(p => p.NombreProvincia),
            "pais" or "nombrepais" => filter.SortDescending ? 
                query.OrderByDescending(p => p.Pais!.NombrePais) : 
                query.OrderBy(p => p.Pais!.NombrePais),
            "fecha" or "fechacreacion" => filter.SortDescending ? 
                query.OrderByDescending(p => p.FechaCreacion) : 
                query.OrderBy(p => p.FechaCreacion),
            "estado" or "estadoactivo" => filter.SortDescending ? 
                query.OrderByDescending(p => p.EstadoActivo) : 
                query.OrderBy(p => p.EstadoActivo),
            _ => filter.SortDescending ? 
                query.OrderByDescending(p => p.NombreProvincia) : 
                query.OrderBy(p => p.NombreProvincia)
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        var provincias = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Include(p => p.Pais)
            .Include(p => p.Ubicaciones.Where(u => u.EstadoActivo)) // Solo ubicaciones activas para el conteo
            .Select(p => new ProvinciaListDto
            {
                Id = p.Id,
                NombreProvincia = p.NombreProvincia,
                PaisId = p.PaisId,
                NombrePais = p.Pais!.NombrePais,
                FechaCreacion = p.FechaCreacion,
                EstadoActivo = p.EstadoActivo,
                FechaActualizacion = p.FechaActualizacion,
                TotalUbicaciones = p.Ubicaciones.Count(u => u.EstadoActivo)
            })
            .ToListAsync();

        return new PaginatedProvinciasDto
        {
            Provincias = provincias,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            HasPrevious = filter.Page > 1,
            HasNext = filter.Page < totalPages
        };
    }

    public async Task<List<Provincia>> GetAllActiveAsync()
    {
        return await _context.Provincias
            .Where(p => p.EstadoActivo)
            .Include(p => p.Pais)
            .OrderBy(p => p.Pais!.NombrePais)
            .ThenBy(p => p.NombreProvincia)
            .ToListAsync();
    }

    public async Task<List<Provincia>> GetByPaisIdAsync(int paisId, bool includeInactive = false)
    {
        var query = _context.Provincias.Where(p => p.PaisId == paisId);
        
        if (!includeInactive)
        {
            query = query.Where(p => p.EstadoActivo);
        }
        
        return await query
            .OrderBy(p => p.NombreProvincia)
            .ToListAsync();
    }

    public async Task<List<Provincia>> GetAllAsync()
    {
        return await _context.Provincias
            .Include(p => p.Pais)
            .OrderBy(p => p.Pais!.NombrePais)
            .ThenBy(p => p.NombreProvincia)
            .ToListAsync();
    }

    // ✏️ OPERACIONES CRUD
    public async Task<Provincia> CreateAsync(Provincia provincia)
    {
        provincia.FechaCreacion = DateTime.Now;
        provincia.EstadoActivo = true;
        
        _context.Provincias.Add(provincia);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🏞️ Provincia creada: {NombreProvincia} (ID: {Id})", provincia.NombreProvincia, provincia.Id);
        return provincia;
    }

    public async Task<Provincia> UpdateAsync(Provincia provincia)
    {
        provincia.FechaActualizacion = DateTime.Now;
        
        _context.Provincias.Update(provincia);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🏞️ Provincia actualizada: {NombreProvincia} (ID: {Id})", provincia.NombreProvincia, provincia.Id);
        return provincia;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var provincia = await GetByIdAsync(id);
        if (provincia == null) return false;

        provincia.EstadoActivo = false;
        provincia.FechaActualizacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🗑️ Provincia eliminada lógicamente: {NombreProvincia} (ID: {Id})", provincia.NombreProvincia, id);
        return true;
    }

    public async Task<bool> RestoreAsync(int id)
    {
        var provincia = await GetByIdAsync(id);
        if (provincia == null) return false;

        provincia.EstadoActivo = true;
        provincia.FechaActualizacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🔄 Provincia restaurada: {NombreProvincia} (ID: {Id})", provincia.NombreProvincia, id);
        return true;
    }

    // ✅ VALIDACIONES
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Provincias.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsByNameInPaisAsync(string nombreProvincia, int paisId, int? excludeId = null)
    {
        var query = _context.Provincias.Where(p => p.NombreProvincia.ToLower() == nombreProvincia.ToLower() 
                                                 && p.PaisId == paisId);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> HasActiveUbicacionesAsync(int provinciaId)
    {
        return await _context.Ubicaciones
            .AnyAsync(u => u.ProvinciaId == provinciaId && u.EstadoActivo);
    }

    // 📊 ESTADÍSTICAS
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Provincias.CountAsync();
    }

    public async Task<int> GetActiveCountAsync()
    {
        return await _context.Provincias.CountAsync(p => p.EstadoActivo);
    }

    public async Task<int> GetInactiveCountAsync()
    {
        return await _context.Provincias.CountAsync(p => !p.EstadoActivo);
    }

    public async Task<int> GetCountByPaisAsync(int paisId)
    {
        return await _context.Provincias.CountAsync(p => p.PaisId == paisId && p.EstadoActivo);
    }

    public async Task<List<Provincia>> GetRecentAsync(int count = 10)
    {
        return await _context.Provincias
            .Where(p => p.EstadoActivo)
            .Include(p => p.Pais)
            .OrderByDescending(p => p.FechaCreacion)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetStatsAsync()
    {
        var stats = new Dictionary<string, int>
        {
            ["Total"] = await GetTotalCountAsync(),
            ["Activos"] = await GetActiveCountAsync(),
            ["Inactivos"] = await GetInactiveCountAsync()
        };

        return stats;
    }
}