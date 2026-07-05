using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

/// <summary>
/// Repository for Fosil entities - handles database connection and mapping
/// Inherits from BaseElementoGeologicoRepository for common operations
/// </summary>
public class FosilRepository : BaseElementoGeologicoRepository<Fosil>, IFosilRepository
{
    public FosilRepository(
        SistemaGeologicoDbContext context, 
        ILogger<BaseElementoGeologicoRepository<Fosil>> logger) 
        : base(context, logger)
    {
    }

    protected override string ElementTypeName => "Fósil";

    protected override Expression<Func<ElementoGeologico, bool>> TypeFilter => 
        e => e is Fosil;

    protected override IQueryable<Fosil> ApplyTypeSpecificFilters(
        IQueryable<Fosil> query, 
        ElementoGeologicoFilterDto filter)
    {
        // Apply fossil-specific filters
        if (filter.TipoFosil.HasValue)
        {
            query = query.Where(f => f.TipoFosil == filter.TipoFosil.Value);
        }

        if (!string.IsNullOrEmpty(filter.Especie))
        {
            query = query.Where(f => f.Especie.Contains(filter.Especie));
        }

        if (!string.IsNullOrEmpty(filter.Periodo))
        {
            query = query.Where(f => f.Periodo.Contains(filter.Periodo));
        }

        return query;
    }

    protected override IQueryable<Fosil> ApplySorting(
        IQueryable<Fosil> query, 
        ElementoGeologicoFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "nombre" => filter.SortDescending ? 
                query.OrderByDescending(f => f.Nombre) : 
                query.OrderBy(f => f.Nombre),
            "codigo" => filter.SortDescending ? 
                query.OrderByDescending(f => f.Codigo) : 
                query.OrderBy(f => f.Codigo),
            "edad" => filter.SortDescending ? 
                query.OrderByDescending(f => f.Edad) : 
                query.OrderBy(f => f.Edad),
            "donante" => filter.SortDescending ? 
                query.OrderByDescending(f => f.Donante) : 
                query.OrderBy(f => f.Donante),
            "fechaingreso" => filter.SortDescending ? 
                query.OrderByDescending(f => f.FechaIngreso) : 
                query.OrderBy(f => f.FechaIngreso),
            "especie" => filter.SortDescending ? 
                query.OrderByDescending(f => f.Especie) : 
                query.OrderBy(f => f.Especie),
            "periodo" => filter.SortDescending ? 
                query.OrderByDescending(f => f.Periodo) : 
                query.OrderBy(f => f.Periodo),
            "tipofosil" => filter.SortDescending ? 
                query.OrderByDescending(f => f.TipoFosil) : 
                query.OrderBy(f => f.TipoFosil),
            _ => filter.SortDescending ? 
                query.OrderByDescending(f => f.Nombre) : 
                query.OrderBy(f => f.Nombre)
        };
    }

    protected override ElementoGeologicoListDto MapToListDto(Fosil fosil)
    {
        return new ElementoGeologicoListDto
        {
            Id = fosil.Id,
            Nombre = fosil.Nombre,
            Codigo = fosil.Codigo,
            TipoElemento = "Fosil",
            Edad = fosil.Edad,
            Donante = fosil.Donante,
            FechaIngreso = fosil.FechaIngreso,
            Ejemplares = fosil.Ejemplares,
            LaminaExiste = fosil.LaminaExiste,
            EstadoActivo = fosil.EstadoActivo,
            FechaCreacion = fosil.FechaCreacion,
            FechaActualizacion = fosil.FechaActualizacion,
            
            // Ubicacion info
            UbicacionId = fosil.UbicacionId,
            Localidad = fosil.Ubicacion?.Localidad,
            NombrePais = fosil.Ubicacion?.Pais?.NombrePais,
            NombreProvincia = fosil.Ubicacion?.Provincia?.NombreProvincia,
            Latitud = fosil.Ubicacion?.Latitud,
            Longitud = fosil.Ubicacion?.Longitud,
            
            // Type-specific fields
            TipoEspecifico = fosil.TipoFosil.ToString(),
            Especie = fosil.Especie,
            Periodo = fosil.Periodo,
            Litologia = null,
            
            // Gallery info
            TotalFotos = fosil.Galeria?.Fotos?.Count ?? 0,
            TieneGaleria = fosil.Galeria != null
        };
    }

    protected override async Task<(Dictionary<string, int> PorTipo, Dictionary<string, int> PorEstado)> GetEstadisticasAsync(ElementoGeologicoFilterDto filter)
    {
        var query = _context.ElementosGeologicos.OfType<Fosil>().AsQueryable();

        // Apply the same filters as the main query
        if (!filter.IncludeInactive)
        {
            query = query.Where(e => e.EstadoActivo);
        }

        // Get type statistics
        var tipoStats = await query
            .GroupBy(f => "Fosil")
            .Select(g => new { Tipo = g.Key, Count = g.Count() })
            .ToListAsync();

        // Get status statistics
        var estadoStats = await query
            .GroupBy(f => f.EstadoActivo ? "Activo" : "Inactivo")
            .Select(g => new { Estado = g.Key, Count = g.Count() })
            .ToListAsync();

        return (
            PorTipo: tipoStats.ToDictionary(x => x.Tipo, x => x.Count),
            PorEstado: estadoStats.ToDictionary(x => x.Estado, x => x.Count)
        );
    }

    // 🦕 FOSSIL-SPECIFIC METHODS
    public async Task<IEnumerable<Fosil>> GetByPeriodoAsync(string periodo)
    {
        return await _context.ElementosGeologicos
            .OfType<Fosil>()
            .Where(f => f.EstadoActivo && f.Periodo.ToLower().Contains(periodo.ToLower()))
            .OrderBy(f => f.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Fosil>> GetByEspecieAsync(string especie)
    {
        return await _context.ElementosGeologicos
            .OfType<Fosil>()
            .Where(f => f.EstadoActivo && f.Especie.ToLower().Contains(especie.ToLower()))
            .OrderBy(f => f.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Fosil>> GetByTipoFosilAsync(SubtipoFosil tipoFosil)
    {
        return await _context.ElementosGeologicos
            .OfType<Fosil>()
            .Where(f => f.EstadoActivo && f.TipoFosil == tipoFosil)
            .OrderBy(f => f.Nombre)
            .ToListAsync();
    }
}