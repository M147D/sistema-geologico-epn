using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Galeria;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class FotoElementoRepository : IFotoElementoRepository
{
    private readonly SistemaGeologicoDbContext _context;

    public FotoElementoRepository(SistemaGeologicoDbContext context)
    {
        _context = context;
    }

    public async Task<FotoElemento?> GetByIdAsync(int id)
    {
        return await _context.FotosElementos
            .Include(f => f.Galeria)
            .ThenInclude(g => g!.ElementoGeologico)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    // Política única de borrado lógico (PEN-12): fuera de Admin, solo fotos activas de
    // galerías y elementos activos.
    public async Task<IEnumerable<FotoElemento>> GetByGaleriaIdAsync(int galeriaId, bool incluirInactivos = false)
    {
        return await _context.FotosElementos
            .AsNoTracking()
            .Where(f => f.GaleriaElementosGeologicoId == galeriaId)
            .Where(f => incluirInactivos || (f.EstadoActivo && f.Galeria!.EstadoActivo && f.Galeria.ElementoGeologico!.EstadoActivo))
            .Select(FotoProyecciones.SinImagen)
            .ToListAsync();
    }

    public async Task<FotoElemento> CreateAsync(FotoElemento foto)
    {
        await _context.FotosElementos.AddAsync(foto);
        await _context.SaveChangesAsync();
        return foto;
    }

    public async Task<bool> EsVisibleAsync(int id) =>
        await _context.FotosElementos.AnyAsync(f => f.Id == id && f.EstadoActivo
            && f.Galeria!.EstadoActivo && f.Galeria.ElementoGeologico!.EstadoActivo);

    public async Task<bool> GaleriaActivaAsync(int galeriaId) =>
        await _context.GaleriaElementosGeologicos.AnyAsync(g => g.Id == galeriaId && g.EstadoActivo
            && g.ElementoGeologico!.EstadoActivo);

    // La entidad ya viene rastreada: SaveChanges escribe solo las columnas que cambiaron,
    // y el token xmin hace fallar la escritura si la fila cambió desde la lectura (PEN-12).
    public async Task<FotoElemento> UpdateAsync(FotoElemento foto)
    {
        if (_context.Entry(foto).State == EntityState.Detached)
            _context.FotosElementos.Update(foto);
        await GuardarAsync();
        return foto;
    }

    public async Task DeleteAsync(int id, int? usuarioId = null)
    {
        var foto = await _context.FotosElementos.FindAsync(id);
        if (foto != null)
        {
            foto.EstadoActivo = false;
            foto.FechaEliminacion = DateTime.Now;
            foto.UsuarioEliminacionId = usuarioId;
            foto.FechaActualizacion = DateTime.Now;
            await GuardarAsync();
        }
    }

    public async Task RestoreAsync(int id, int? usuarioId = null)
    {
        var foto = await _context.FotosElementos.FindAsync(id);
        if (foto != null)
        {
            foto.EstadoActivo = true;
            foto.FechaEliminacion = null;
            foto.UsuarioEliminacionId = null;
            foto.FechaActualizacion = DateTime.Now;
            foto.UsuarioActualizacionId = usuarioId;
            await GuardarAsync();
        }
    }

    private async Task GuardarAsync()
    {
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException ex) { throw new ConflictoConcurrenciaException(ex); }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.FotosElementos.AnyAsync(f => f.Id == id);
    }

    public async Task<int?> GetOrCreateGaleriaIdAsync(int elementoId)
    {
        var elemento = await _context.ElementosGeologicos
            .Include(e => e.Galeria)
            .FirstOrDefaultAsync(e => e.Id == elementoId && e.EstadoActivo);

        if (elemento == null) return null;

        if (elemento.Galeria == null)
        {
            var galeria = new GaleriaElementoGeologico
            {
                ElementoGeologicoId = elementoId,
                DetalleGrupo = "Galeria",
                FechaCreacion = DateTime.Now,
                EstadoActivo = true
            };
            _context.GaleriaElementosGeologicos.Add(galeria);
            await _context.SaveChangesAsync();
            return galeria.Id;
        }

        return elemento.Galeria.Id;
    }

    public async Task<(int? galeriaId, IEnumerable<FotoElemento> fotos)?> GetGaleriaConFotosAsync(int elementoId, bool soloActivos)
    {
        var elemento = await _context.ElementosGeologicos
            .AsNoTracking()
            .Include(e => e.Galeria)
            .FirstOrDefaultAsync(e => e.Id == elementoId && e.EstadoActivo);

        if (elemento == null) return null;

        if (elemento.Galeria == null)
            return ((int?)null, Enumerable.Empty<FotoElemento>());

        // Solo metadatos: la columna imagen no se lee (REV-01, H-09).
        var fotos = await _context.FotosElementos
            .AsNoTracking()
            .Where(f => f.GaleriaElementosGeologicoId == elemento.Galeria.Id && (!soloActivos || f.EstadoActivo))
            .Select(FotoProyecciones.SinImagen)
            .ToListAsync();

        return ((int?)elemento.Galeria.Id, fotos);
    }
}