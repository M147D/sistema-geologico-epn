using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class ElementoGeologicoRepository : IElementoGeologicoRepository
{
    private readonly GestorSistemaGeologia _context;
    private readonly ILogger<ElementoGeologicoRepository> _logger;

    public ElementoGeologicoRepository(GestorSistemaGeologia context, ILogger<ElementoGeologicoRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 🔍 CONSULTAS GENERALES
    public async Task<ElementoGeologico?> GetByIdAsync(int id)
    {
        return await _context.ElementosGeologicos
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<ElementoGeologico?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.ElementosGeologicos
            .Include(e => e.Ubicacion)
                .ThenInclude(u => u.Pais)
            .Include(e => e.Ubicacion)
                .ThenInclude(u => u.Provincia)
            .Include(e => e.Galeria)
                .ThenInclude(g => g.Fotos)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<ElementoGeologico?> GetByCodigoAsync(string codigo)
    {
        return await _context.ElementosGeologicos
            .FirstOrDefaultAsync(e => e.Codigo == codigo);
    }

    public async Task<PaginatedElementosGeologicosDto> GetAllAsync(ElementoGeologicoFilterDto filter)
    {
        var query = _context.ElementosGeologicos.AsQueryable();

        // 🔥 POR DEFECTO: Solo mostrar elementos activos
        if (!filter.IncludeInactive)
        {
            query = query.Where(e => e.EstadoActivo);
        }

        // Aplicar filtros básicos
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

        if (!string.IsNullOrEmpty(filter.Edad))
        {
            query = query.Where(e => e.Edad.Contains(filter.Edad));
        }

        // Filtros por tipo de elemento
        if (!string.IsNullOrEmpty(filter.TipoElemento))
        {
            query = query.Where(e => EF.Property<string>(e, "TipoElemento") == filter.TipoElemento);
        }

        // Filtros por ubicación
        if (filter.UbicacionId.HasValue)
        {
            query = query.Where(e => e.UbicacionId == filter.UbicacionId.Value);
        }

        if (filter.PaisId.HasValue)
        {
            query = query.Where(e => e.Ubicacion.PaisId == filter.PaisId.Value);
        }

        if (filter.ProvinciaId.HasValue)
        {
            query = query.Where(e => e.Ubicacion.ProvinciaId == filter.ProvinciaId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Localidad))
        {
            query = query.Where(e => e.Ubicacion.Localidad.Contains(filter.Localidad));
        }

        // Filtros por fechas
        if (filter.FechaIngresoDesde.HasValue)
        {
            query = query.Where(e => e.FechaIngreso >= filter.FechaIngresoDesde.Value);
        }

        if (filter.FechaIngresoHasta.HasValue)
        {
            query = query.Where(e => e.FechaIngreso <= filter.FechaIngresoHasta.Value);
        }

        if (filter.FechaCreacionDesde.HasValue)
        {
            query = query.Where(e => e.FechaCreacion >= filter.FechaCreacionDesde.Value);
        }

        if (filter.FechaCreacionHasta.HasValue)
        {
            query = query.Where(e => e.FechaCreacion <= filter.FechaCreacionHasta.Value);
        }

        // Filtros específicos
        if (filter.LaminaExiste.HasValue)
        {
            query = query.Where(e => e.LaminaExiste == filter.LaminaExiste.Value);
        }

        if (filter.EjemplaresMin.HasValue)
        {
            query = query.Where(e => e.Ejemplares >= filter.EjemplaresMin.Value);
        }

        if (filter.EjemplaresMax.HasValue)
        {
            query = query.Where(e => e.Ejemplares <= filter.EjemplaresMax.Value);
        }

        // Filtros por estado
        if (filter.EstadoActivo.HasValue)
        {
            query = query.Where(e => e.EstadoActivo == filter.EstadoActivo.Value);
        }

        // Filtros específicos por tipo de elemento
        if (filter.TipoFosil.HasValue)
        {
            query = query.OfType<Fosil>().Where(f => f.TipoFosil == filter.TipoFosil.Value);
        }

        if (filter.TipoMineral.HasValue)
        {
            query = query.OfType<Mineral>().Where(m => m.TipoMineral == filter.TipoMineral.Value);
        }

        if (filter.TipoRoca.HasValue)
        {
            query = query.OfType<Roca>().Where(r => r.TipoRoca == filter.TipoRoca.Value);
        }

        if (!string.IsNullOrEmpty(filter.Especie))
        {
            query = query.OfType<Fosil>().Where(f => f.Especie.Contains(filter.Especie));
        }

        if (!string.IsNullOrEmpty(filter.Periodo))
        {
            query = query.OfType<Fosil>().Where(f => f.Periodo.Contains(filter.Periodo));
        }

        if (!string.IsNullOrEmpty(filter.Litologia))
        {
            query = query.Where(e => (e is Mineral && ((Mineral)e).Litologia.Contains(filter.Litologia)) ||
                                   (e is Roca && ((Roca)e).Litologia.Contains(filter.Litologia)));
        }

        // Incluir relaciones si se solicita
        if (filter.IncludeUbicacion)
        {
            query = query.Include(e => e.Ubicacion)
                         .ThenInclude(u => u.Pais)
                         .Include(e => e.Ubicacion)
                         .ThenInclude(u => u.Provincia);
        }

        if (filter.IncludeGaleria)
        {
            query = query.Include(e => e.Galeria);
            
            if (filter.IncludeFotos)
            {
                query = query.Include(e => e.Galeria)
                             .ThenInclude(g => g.Fotos);
            }
        }

        // Aplicar ordenamiento
        query = filter.SortBy?.ToLower() switch
        {
            "nombre" => filter.SortDescending ? 
                query.OrderByDescending(e => e.Nombre) : 
                query.OrderBy(e => e.Nombre),
            "codigo" => filter.SortDescending ? 
                query.OrderByDescending(e => e.Codigo) : 
                query.OrderBy(e => e.Codigo),
            "edad" => filter.SortDescending ? 
                query.OrderByDescending(e => e.Edad) : 
                query.OrderBy(e => e.Edad),
            "donante" => filter.SortDescending ? 
                query.OrderByDescending(e => e.Donante) : 
                query.OrderBy(e => e.Donante),
            "fechaingreso" => filter.SortDescending ? 
                query.OrderByDescending(e => e.FechaIngreso) : 
                query.OrderBy(e => e.FechaIngreso),
            "fechacreacion" => filter.SortDescending ? 
                query.OrderByDescending(e => e.FechaCreacion) : 
                query.OrderBy(e => e.FechaCreacion),
            "ejemplares" => filter.SortDescending ? 
                query.OrderByDescending(e => e.Ejemplares) : 
                query.OrderBy(e => e.Ejemplares),
            _ => filter.SortDescending ? 
                query.OrderByDescending(e => e.Nombre) : 
                query.OrderBy(e => e.Nombre)
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        var elementos = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(e => new ElementoGeologicoListDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Codigo = e.Codigo,
                TipoElemento = EF.Property<string>(e, "TipoElemento"),
                Edad = e.Edad,
                Donante = e.Donante,
                FechaIngreso = e.FechaIngreso,
                Ejemplares = e.Ejemplares,
                LaminaExiste = e.LaminaExiste,
                EstadoActivo = e.EstadoActivo,
                FechaCreacion = e.FechaCreacion,
                FechaActualizacion = e.FechaActualizacion,
                
                // Información de ubicación
                Localidad = e.Ubicacion != null ? e.Ubicacion.Localidad : null,
                NombrePais = e.Ubicacion != null && e.Ubicacion.Pais != null ? e.Ubicacion.Pais.NombrePais : null,
                NombreProvincia = e.Ubicacion != null && e.Ubicacion.Provincia != null ? e.Ubicacion.Provincia.NombreProvincia : null,
                Latitud = e.Ubicacion != null ? e.Ubicacion.Latitud : null,
                Longitud = e.Ubicacion != null ? e.Ubicacion.Longitud : null,
                
                // Información específica por tipo
                TipoEspecifico = e is Fosil ? ((Fosil)e).TipoFosil.ToString() :
                               e is Mineral ? ((Mineral)e).TipoMineral.ToString() :
                               e is Roca ? ((Roca)e).TipoRoca.ToString() : null,
                Especie = e is Fosil ? ((Fosil)e).Especie : null,
                Periodo = e is Fosil ? ((Fosil)e).Periodo : null,
                Litologia = e is Mineral ? ((Mineral)e).Litologia :
                          e is Roca ? ((Roca)e).Litologia : null,
                
                // Información de galería
                TotalFotos = e.Galeria != null ? e.Galeria.Fotos.Count : 0,
                TieneGaleria = e.Galeria != null
            })
            .ToListAsync();

        // Estadísticas por tipo
        var tipoStats = await query
            .GroupBy(e => EF.Property<string>(e, "TipoElemento"))
            .Select(g => new { Tipo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Tipo, x => x.Count);

        // Estadísticas por estado
        var estadoStats = await query
            .GroupBy(e => e.EstadoActivo)
            .Select(g => new { Estado = g.Key ? "Activo" : "Inactivo", Count = g.Count() })
            .ToDictionaryAsync(x => x.Estado, x => x.Count);

        return new PaginatedElementosGeologicosDto
        {
            ElementosGeologicos = elementos,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            HasPrevious = filter.Page > 1,
            HasNext = filter.Page < totalPages,
            TipoStats = tipoStats,
            EstadoStats = estadoStats
        };
    }

    public async Task<List<ElementoGeologico>> GetAllActiveAsync()
    {
        return await _context.ElementosGeologicos
            .Where(e => e.EstadoActivo)
            .OrderBy(e => e.Nombre)
            .ToListAsync();
    }

    public async Task<List<ElementoGeologico>> GetAllAsync()
    {
        return await _context.ElementosGeologicos
            .OrderBy(e => e.Nombre)
            .ToListAsync();
    }

    // 🔍 CONSULTAS ESPECÍFICAS POR TIPO
    public async Task<T?> GetByIdAsync<T>(int id) where T : ElementoGeologico
    {
        return await _context.Set<T>()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<T>> GetByTypeAsync<T>(ElementoGeologicoFilterDto? filter = null) where T : ElementoGeologico
    {
        var query = _context.Set<T>().AsQueryable();
        
        if (filter != null && !filter.IncludeInactive)
        {
            query = query.Where(e => e.EstadoActivo);
        }
        
        return await query.ToListAsync();
    }

    public async Task<PaginatedElementosGeologicosDto> GetFosilesAsync(ElementoGeologicoFilterDto filter)
    {
        filter.TipoElemento = "Fosil";
        return await GetAllAsync(filter);
    }

    public async Task<PaginatedElementosGeologicosDto> GetMineralesAsync(ElementoGeologicoFilterDto filter)
    {
        filter.TipoElemento = "Mineral";
        return await GetAllAsync(filter);
    }

    public async Task<PaginatedElementosGeologicosDto> GetRocasAsync(ElementoGeologicoFilterDto filter)
    {
        filter.TipoElemento = "Roca";
        return await GetAllAsync(filter);
    }

    // 🔍 CONSULTAS POR UBICACIÓN
    public async Task<List<ElementoGeologico>> GetByUbicacionAsync(int ubicacionId)
    {
        return await _context.ElementosGeologicos
            .Where(e => e.UbicacionId == ubicacionId && e.EstadoActivo)
            .ToListAsync();
    }

    public async Task<List<ElementoGeologico>> GetByPaisAsync(int paisId)
    {
        return await _context.ElementosGeologicos
            .Where(e => e.Ubicacion.PaisId == paisId && e.EstadoActivo)
            .ToListAsync();
    }

    public async Task<List<ElementoGeologico>> GetByProvinciaAsync(int provinciaId)
    {
        return await _context.ElementosGeologicos
            .Where(e => e.Ubicacion.ProvinciaId == provinciaId && e.EstadoActivo)
            .ToListAsync();
    }

    // ✏️ OPERACIONES CRUD GENERALES
    public async Task<ElementoGeologico> CreateAsync(ElementoGeologico elemento)
    {
        elemento.FechaCreacion = DateTime.Now;
        elemento.EstadoActivo = true;
        
        _context.ElementosGeologicos.Add(elemento);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🪨 Elemento geológico creado: {Nombre} (ID: {Id}, Tipo: {Tipo})", 
            elemento.Nombre, elemento.Id, elemento.GetType().Name);
        return elemento;
    }

    public async Task<ElementoGeologico> UpdateAsync(ElementoGeologico elemento)
    {
        elemento.FechaActualizacion = DateTime.Now;
        
        _context.ElementosGeologicos.Update(elemento);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🪨 Elemento geológico actualizado: {Nombre} (ID: {Id})", 
            elemento.Nombre, elemento.Id);
        return elemento;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var elemento = await GetByIdAsync(id);
        if (elemento == null) return false;

        elemento.EstadoActivo = false;
        elemento.FechaActualizacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🗑️ Elemento geológico eliminado lógicamente: {Nombre} (ID: {Id})", 
            elemento.Nombre, id);
        return true;
    }

    public async Task<bool> RestoreAsync(int id)
    {
        var elemento = await GetByIdAsync(id);
        if (elemento == null) return false;

        elemento.EstadoActivo = true;
        elemento.FechaActualizacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🔄 Elemento geológico restaurado: {Nombre} (ID: {Id})", 
            elemento.Nombre, id);
        return true;
    }

    // ✏️ OPERACIONES CRUD ESPECÍFICAS POR TIPO
    public async Task<Fosil> CreateFosilAsync(Fosil fosil)
    {
        fosil.FechaCreacion = DateTime.Now;
        fosil.EstadoActivo = true;
        
        _context.Fosiles.Add(fosil);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🦕 Fósil creado: {Nombre} (ID: {Id}, Tipo: {Tipo})", 
            fosil.Nombre, fosil.Id, fosil.TipoFosil);
        return fosil;
    }

    public async Task<Mineral> CreateMineralAsync(Mineral mineral)
    {
        mineral.FechaCreacion = DateTime.Now;
        mineral.EstadoActivo = true;
        
        _context.Minerales.Add(mineral);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("💎 Mineral creado: {Nombre} (ID: {Id}, Tipo: {Tipo})", 
            mineral.Nombre, mineral.Id, mineral.TipoMineral);
        return mineral;
    }

    public async Task<Roca> CreateRocaAsync(Roca roca)
    {
        roca.FechaCreacion = DateTime.Now;
        roca.EstadoActivo = true;
        
        _context.Rocas.Add(roca);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🪨 Roca creada: {Nombre} (ID: {Id}, Tipo: {Tipo})", 
            roca.Nombre, roca.Id, roca.TipoRoca);
        return roca;
    }

    public async Task<Fosil> UpdateFosilAsync(Fosil fosil)
    {
        fosil.FechaActualizacion = DateTime.Now;
        
        _context.Fosiles.Update(fosil);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🦕 Fósil actualizado: {Nombre} (ID: {Id})", fosil.Nombre, fosil.Id);
        return fosil;
    }

    public async Task<Mineral> UpdateMineralAsync(Mineral mineral)
    {
        mineral.FechaActualizacion = DateTime.Now;
        
        _context.Minerales.Update(mineral);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("💎 Mineral actualizado: {Nombre} (ID: {Id})", mineral.Nombre, mineral.Id);
        return mineral;
    }

    public async Task<Roca> UpdateRocaAsync(Roca roca)
    {
        roca.FechaActualizacion = DateTime.Now;
        
        _context.Rocas.Update(roca);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🪨 Roca actualizada: {Nombre} (ID: {Id})", roca.Nombre, roca.Id);
        return roca;
    }

    // 🏢 OPERACIONES CON UBICACIONES
    public async Task<Ubicacion> CreateUbicacionAsync(Ubicacion ubicacion)
    {
        // Ensure no navigation properties are being tracked to avoid FK issues
        ubicacion.Pais = null;
        ubicacion.Provincia = null;
        
        _context.Ubicaciones.Add(ubicacion);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("📍 Ubicación creada: {Localidad} (ID: {Id})", ubicacion.Localidad, ubicacion.Id);
        return ubicacion;
    }

    // ✅ VALIDACIONES
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.ElementosGeologicos.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
    {
        var query = _context.ElementosGeologicos.Where(e => e.Codigo == codigo);
        
        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> HasActiveUbicacionAsync(int elementoId)
    {
        var elemento = await _context.ElementosGeologicos
            .Include(e => e.Ubicacion)
            .FirstOrDefaultAsync(e => e.Id == elementoId);
            
        return elemento?.Ubicacion?.EstadoActivo == true;
    }

    public async Task<bool> HasGaleriaAsync(int elementoId)
    {
        return await _context.GaleriaElementosGeologicos
            .AnyAsync(g => g.ElementoGeologicoId == elementoId);
    }

    public async Task<bool> HasFotosAsync(int elementoId)
    {
        return await _context.FotosElementos
            .Include(f => f.Galeria)
            .AnyAsync(f => f.Galeria.ElementoGeologicoId == elementoId);
    }

    // 📊 ESTADÍSTICAS
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.ElementosGeologicos.CountAsync();
    }

    public async Task<int> GetActiveCountAsync()
    {
        return await _context.ElementosGeologicos.CountAsync(e => e.EstadoActivo);
    }

    public async Task<int> GetInactiveCountAsync()
    {
        return await _context.ElementosGeologicos.CountAsync(e => !e.EstadoActivo);
    }

    public async Task<Dictionary<string, int>> GetCountByTypeAsync()
    {
        return await _context.ElementosGeologicos
            .Where(e => e.EstadoActivo)
            .GroupBy(e => EF.Property<string>(e, "TipoElemento"))
            .Select(g => new { Tipo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Tipo, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetCountByUbicacionAsync()
    {
        return await _context.ElementosGeologicos
            .Where(e => e.EstadoActivo && e.Ubicacion.EstadoActivo)
            .Include(e => e.Ubicacion)
                .ThenInclude(u => u.Pais)
            .GroupBy(e => e.Ubicacion.Pais.NombrePais)
            .Select(g => new { Pais = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Pais, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetStatsAsync()
    {
        var stats = new Dictionary<string, int>
        {
            ["Total"] = await GetTotalCountAsync(),
            ["Activos"] = await GetActiveCountAsync(),
            ["Inactivos"] = await GetInactiveCountAsync(),
            ["Fosiles"] = await _context.Fosiles.CountAsync(f => f.EstadoActivo),
            ["Minerales"] = await _context.Minerales.CountAsync(m => m.EstadoActivo),
            ["Rocas"] = await _context.Rocas.CountAsync(r => r.EstadoActivo)
        };

        return stats;
    }

    public async Task<List<ElementoGeologico>> GetRecentAsync(int count = 10)
    {
        return await _context.ElementosGeologicos
            .Where(e => e.EstadoActivo)
            .OrderByDescending(e => e.FechaCreacion)
            .Take(count)
            .ToListAsync();
    }

    // 📈 REPORTES
    public async Task<Dictionary<string, object>> GetDashboardStatsAsync()
    {
        var stats = new Dictionary<string, object>();
        
        stats["TotalElementos"] = await GetActiveCountAsync();
        stats["ElementosPorTipo"] = await GetCountByTypeAsync();
        stats["ElementosPorPais"] = await GetCountByUbicacionAsync();
        
        var elementosRecientes = await GetRecentAsync(5);
        stats["ElementosRecientes"] = elementosRecientes.Select(e => new 
        {
            e.Id,
            e.Nombre,
            e.Codigo,
            Tipo = e.GetType().Name,
            e.FechaCreacion
        }).ToList();
        
        return stats;
    }

    public async Task<List<ElementoGeologico>> GetElementosByDateRangeAsync(DateTime desde, DateTime hasta)
    {
        return await _context.ElementosGeologicos
            .Where(e => e.EstadoActivo && e.FechaCreacion >= desde && e.FechaCreacion <= hasta)
            .OrderBy(e => e.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<ElementoGeologico>> GetElementosByDonanteAsync(string donante)
    {
        return await _context.ElementosGeologicos
            .Where(e => e.EstadoActivo && e.Donante.Contains(donante))
            .OrderBy(e => e.Nombre)
            .ToListAsync();
    }

    // 🔄 HISTORIAL DE ACCESO
    public async Task RegisterAccessAsync(int elementoId, int usuarioId, AccionesUsuario accion)
    {
        // Para visualización, verificar si ya existe un registro reciente y actualizarlo
        if (accion == AccionesUsuario.Visualizacion)
        {
            await UpdateOrCreateVisualizacionAsync(elementoId, usuarioId);
            return;
        }

        // Para otras acciones, siempre crear un nuevo registro
        var historial = new HistorialAcceso
        {
            ElementoGeologicoId = elementoId,
            UsuarioId = usuarioId,
            Accion = accion,
            FechaAcceso = DateTime.UtcNow
        };

        _context.HistorialAccesos.Add(historial);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("📝 Acción registrada: {Accion} en elemento {ElementoId} por usuario {UsuarioId}", 
            accion, elementoId, usuarioId);
    }

    public async Task<List<HistorialAcceso>> GetHistorialAsync(int elementoId)
    {
        return await _context.HistorialAccesos
            .Where(h => h.ElementoGeologicoId == elementoId)
            .Include(h => h.Usuario)
            .OrderByDescending(h => h.FechaAcceso)
            .ToListAsync();
    }

    public async Task<HistorialAcceso?> GetLastVisualizacionAsync(int elementoId, int usuarioId)
    {
        return await _context.HistorialAccesos
            .Where(h => h.ElementoGeologicoId == elementoId && 
                       h.UsuarioId == usuarioId && 
                       h.Accion == AccionesUsuario.Visualizacion)
            .OrderByDescending(h => h.FechaAcceso)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateOrCreateVisualizacionAsync(int elementoId, int usuarioId)
    {
        var ultimaVisualizacion = await GetLastVisualizacionAsync(elementoId, usuarioId);
        
        if (ultimaVisualizacion != null)
        {
            // Actualizar la fecha de la última visualización
            ultimaVisualizacion.FechaAcceso = DateTime.UtcNow;
            _context.HistorialAccesos.Update(ultimaVisualizacion);
        }
        else
        {
            // Crear nueva visualización
            var nuevaVisualizacion = new HistorialAcceso
            {
                ElementoGeologicoId = elementoId,
                UsuarioId = usuarioId,
                Accion = AccionesUsuario.Visualizacion,
                FechaAcceso = DateTime.UtcNow
            };
            _context.HistorialAccesos.Add(nuevaVisualizacion);
        }
        
        await _context.SaveChangesAsync();
    }
}
