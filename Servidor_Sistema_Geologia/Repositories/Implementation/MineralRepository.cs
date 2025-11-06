using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

/// <summary>
/// Repository for Mineral entities - handles database connection and mapping
/// Inherits from BaseElementoGeologicoRepository for common operations
/// </summary>
public class MineralRepository : BaseElementoGeologicoRepository<Mineral>, IMineralRepository
{
    public MineralRepository(
        GestorSistemaGeologia context, 
        ILogger<BaseElementoGeologicoRepository<Mineral>> logger) 
        : base(context, logger)
    {
    }

    protected override string ElementTypeName => "Mineral";

    protected override Expression<Func<ElementoGeologico, bool>> TypeFilter => 
        e => e is Mineral;

    protected override IQueryable<Mineral> ApplyTypeSpecificFilters(
        IQueryable<Mineral> query, 
        ElementoGeologicoFilterDto filter)
    {
        // Apply mineral-specific filters
        if (filter.TipoMineral.HasValue)
        {
            query = query.Where(m => m.TipoMineral == filter.TipoMineral.Value);
        }

        if (!string.IsNullOrEmpty(filter.Litologia))
        {
            query = query.Where(m => m.Litologia.Contains(filter.Litologia));
        }

        return query;
    }

    protected override IQueryable<Mineral> ApplySorting(
        IQueryable<Mineral> query, 
        ElementoGeologicoFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "nombre" => filter.SortDescending ? 
                query.OrderByDescending(m => m.Nombre) : 
                query.OrderBy(m => m.Nombre),
            "codigo" => filter.SortDescending ? 
                query.OrderByDescending(m => m.Codigo) : 
                query.OrderBy(m => m.Codigo),
            "edad" => filter.SortDescending ? 
                query.OrderByDescending(m => m.Edad) : 
                query.OrderBy(m => m.Edad),
            "donante" => filter.SortDescending ? 
                query.OrderByDescending(m => m.Donante) : 
                query.OrderBy(m => m.Donante),
            "fechaingreso" => filter.SortDescending ? 
                query.OrderByDescending(m => m.FechaIngreso) : 
                query.OrderBy(m => m.FechaIngreso),
            "litologia" => filter.SortDescending ? 
                query.OrderByDescending(m => m.Litologia) : 
                query.OrderBy(m => m.Litologia),
            "tipomineral" => filter.SortDescending ? 
                query.OrderByDescending(m => m.TipoMineral) : 
                query.OrderBy(m => m.TipoMineral),
            _ => filter.SortDescending ? 
                query.OrderByDescending(m => m.Nombre) : 
                query.OrderBy(m => m.Nombre)
        };
    }

    protected override ElementoGeologicoListDto MapToListDto(Mineral mineral)
    {
        return new ElementoGeologicoListDto
        {
            Id = mineral.Id,
            Nombre = mineral.Nombre,
            Codigo = mineral.Codigo,
            TipoElemento = "Mineral",
            Edad = mineral.Edad,
            Donante = mineral.Donante,
            FechaIngreso = mineral.FechaIngreso,
            Ejemplares = mineral.Ejemplares,
            LaminaExiste = mineral.LaminaExiste,
            EstadoActivo = mineral.EstadoActivo,
            FechaCreacion = mineral.FechaCreacion,
            FechaActualizacion = mineral.FechaActualizacion,
            
            // Ubicacion info
            UbicacionId = mineral.UbicacionId,
            Localidad = mineral.Ubicacion?.Localidad,
            NombrePais = mineral.Ubicacion?.Pais?.NombrePais,
            NombreProvincia = mineral.Ubicacion?.Provincia?.NombreProvincia,
            Latitud = mineral.Ubicacion?.Latitud,
            Longitud = mineral.Ubicacion?.Longitud,
            
            // Type-specific fields
            TipoEspecifico = mineral.TipoMineral.ToString(),
            Especie = null, // Solo para fósiles
            Periodo = null, // Solo para fósiles
            Litologia = mineral.Litologia,
            
            // Gallery info
            TotalFotos = mineral.Galeria?.Fotos?.Count ?? 0,
            TieneGaleria = mineral.Galeria != null
        };
    }

    protected override async Task<(Dictionary<string, int> PorTipo, Dictionary<string, int> PorEstado)> GetEstadisticasAsync(ElementoGeologicoFilterDto filter)
    {
        var query = _context.ElementosGeologicos.OfType<Mineral>().AsQueryable();

        // Apply the same filters as the main query
        if (!filter.IncludeInactive)
        {
            query = query.Where(e => e.EstadoActivo);
        }

        // Get type statistics
        var tipoStats = await query
            .GroupBy(m => "Mineral")
            .Select(g => new { Tipo = g.Key, Count = g.Count() })
            .ToListAsync();

        // Get status statistics
        var estadoStats = await query
            .GroupBy(m => m.EstadoActivo ? "Activo" : "Inactivo")
            .Select(g => new { Estado = g.Key, Count = g.Count() })
            .ToListAsync();

        return (
            PorTipo: tipoStats.ToDictionary(x => x.Tipo, x => x.Count),
            PorEstado: estadoStats.ToDictionary(x => x.Estado, x => x.Count)
        );
    }

    // 💎 MINERAL-SPECIFIC METHODS
    public async Task<IEnumerable<Mineral>> GetByLitologiaAsync(string litologia)
    {
        return await _context.ElementosGeologicos
            .OfType<Mineral>()
            .Where(m => m.EstadoActivo && m.Litologia.ToLower().Contains(litologia.ToLower()))
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Mineral>> GetByTipoMineralAsync(SubtipoMineral tipoMineral)
    {
        return await _context.ElementosGeologicos
            .OfType<Mineral>()
            .Where(m => m.EstadoActivo && m.TipoMineral == tipoMineral)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }
}