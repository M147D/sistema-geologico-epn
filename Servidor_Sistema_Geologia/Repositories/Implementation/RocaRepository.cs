using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

/// <summary>
/// Repository for Roca entities - handles database connection and mapping
/// Inherits from BaseElementoGeologicoRepository for common operations
/// </summary>
public class RocaRepository : BaseElementoGeologicoRepository<Roca>, IRocaRepository
{
    public RocaRepository(
        SistemaGeologicoDbContext context, 
        ILogger<BaseElementoGeologicoRepository<Roca>> logger) 
        : base(context, logger)
    {
    }

    protected override string ElementTypeName => "Roca";

    protected override Expression<Func<ElementoGeologico, bool>> TypeFilter => 
        e => e is Roca;

    protected override IQueryable<Roca> ApplyTypeSpecificFilters(
        IQueryable<Roca> query, 
        ElementoGeologicoFilterDto filter)
    {
        // Apply rock-specific filters
        if (filter.TipoRoca.HasValue)
        {
            query = query.Where(r => r.TipoRoca == filter.TipoRoca.Value);
        }

        if (!string.IsNullOrEmpty(filter.Litologia))
        {
            query = query.Where(r => r.Litologia.Contains(filter.Litologia));
        }

        return query;
    }

    protected override IQueryable<Roca> ApplySorting(
        IQueryable<Roca> query, 
        ElementoGeologicoFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "nombre" => filter.SortDescending ? 
                query.OrderByDescending(r => r.Nombre) : 
                query.OrderBy(r => r.Nombre),
            "codigo" => filter.SortDescending ? 
                query.OrderByDescending(r => r.Codigo) : 
                query.OrderBy(r => r.Codigo),
            "edad" => filter.SortDescending ? 
                query.OrderByDescending(r => r.Edad) : 
                query.OrderBy(r => r.Edad),
            "donante" => filter.SortDescending ? 
                query.OrderByDescending(r => r.Donante) : 
                query.OrderBy(r => r.Donante),
            "fechaingreso" => filter.SortDescending ? 
                query.OrderByDescending(r => r.FechaIngreso) : 
                query.OrderBy(r => r.FechaIngreso),
            "litologia" => filter.SortDescending ? 
                query.OrderByDescending(r => r.Litologia) : 
                query.OrderBy(r => r.Litologia),
            "tiporoca" => filter.SortDescending ? 
                query.OrderByDescending(r => r.TipoRoca) : 
                query.OrderBy(r => r.TipoRoca),
            _ => filter.SortDescending ? 
                query.OrderByDescending(r => r.Nombre) : 
                query.OrderBy(r => r.Nombre)
        };
    }

    protected override ElementoGeologicoListDto MapToListDto(Roca roca)
    {
        return new ElementoGeologicoListDto
        {
            Id = roca.Id,
            Nombre = roca.Nombre,
            Codigo = roca.Codigo,
            TipoElemento = "Roca",
            Edad = roca.Edad,
            Donante = roca.Donante,
            FechaIngreso = roca.FechaIngreso,
            Ejemplares = roca.Ejemplares,
            LaminaExiste = roca.LaminaExiste,
            EstadoActivo = roca.EstadoActivo,
            FechaCreacion = roca.FechaCreacion,
            FechaActualizacion = roca.FechaActualizacion,
            
            // Ubicacion info
            UbicacionId = roca.UbicacionId,
            Localidad = roca.Ubicacion?.Localidad,
            NombrePais = roca.Ubicacion?.Pais?.NombrePais,
            NombreProvincia = roca.Ubicacion?.Provincia?.NombreProvincia,
            Latitud = roca.Ubicacion?.Latitud,
            Longitud = roca.Ubicacion?.Longitud,
            
            // Type-specific fields
            TipoEspecifico = roca.TipoRoca.ToString(),
            Especie = null, // Solo para fósiles
            Periodo = null, // Solo para fósiles
            Litologia = roca.Litologia,
            
            // Gallery info
            TotalFotos = roca.Galeria?.Fotos?.Count ?? 0,
            TieneGaleria = roca.Galeria != null
        };
    }

    protected override async Task<(Dictionary<string, int> PorTipo, Dictionary<string, int> PorEstado)> GetEstadisticasAsync(ElementoGeologicoFilterDto filter)
    {
        var query = _context.ElementosGeologicos.OfType<Roca>().AsQueryable();

        // Apply the same filters as the main query
        if (!filter.IncludeInactive)
        {
            query = query.Where(e => e.EstadoActivo);
        }

        // Get type statistics
        var tipoStats = await query
            .GroupBy(r => "Roca")
            .Select(g => new { Tipo = g.Key, Count = g.Count() })
            .ToListAsync();

        // Get status statistics
        var estadoStats = await query
            .GroupBy(r => r.EstadoActivo ? "Activo" : "Inactivo")
            .Select(g => new { Estado = g.Key, Count = g.Count() })
            .ToListAsync();

        return (
            PorTipo: tipoStats.ToDictionary(x => x.Tipo, x => x.Count),
            PorEstado: estadoStats.ToDictionary(x => x.Estado, x => x.Count)
        );
    }

    // 🪨 ROCK-SPECIFIC METHODS
    public async Task<IEnumerable<Roca>> GetByLitologiaAsync(string litologia)
    {
        return await _context.ElementosGeologicos
            .OfType<Roca>()
            .Where(r => r.EstadoActivo && r.Litologia.ToLower().Contains(litologia.ToLower()))
            .OrderBy(r => r.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Roca>> GetByTipoRocaAsync(SubtipoRoca tipoRoca)
    {
        return await _context.ElementosGeologicos
            .OfType<Roca>()
            .Where(r => r.EstadoActivo && r.TipoRoca == tipoRoca)
            .OrderBy(r => r.Nombre)
            .ToListAsync();
    }
}