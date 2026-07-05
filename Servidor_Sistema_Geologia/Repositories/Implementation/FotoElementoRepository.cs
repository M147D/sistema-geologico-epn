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

    public async Task<IEnumerable<FotoElemento>> GetByGaleriaIdAsync(int galeriaId)
    {
        return await _context.FotosElementos
            .Include(f => f.Galeria)
            .Where(f => f.GaleriaElementosGeologicoId == galeriaId)
            .ToListAsync();
    }

    public async Task<FotoElemento> CreateAsync(FotoElemento foto)
    {
        await _context.FotosElementos.AddAsync(foto);
        await _context.SaveChangesAsync();
        return foto;
    }

    public async Task<FotoElemento> UpdateAsync(FotoElemento foto)
    {
        _context.FotosElementos.Update(foto);
        await _context.SaveChangesAsync();
        return foto;
    }

    public async Task DeleteAsync(int id)
    {
        var foto = await _context.FotosElementos.FindAsync(id);
        if (foto != null)
        {
            foto.EstadoActivo = false;
            foto.FechaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RestoreAsync(int id)
    {
        var foto = await _context.FotosElementos.FindAsync(id);
        if (foto != null)
        {
            foto.EstadoActivo = true;
            foto.FechaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();
        }
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
            .Include(e => e.Galeria)
                .ThenInclude(g => g!.Fotos)
            .FirstOrDefaultAsync(e => e.Id == elementoId && e.EstadoActivo);

        if (elemento == null) return null;

        if (elemento.Galeria == null)
            return ((int?)null, Enumerable.Empty<FotoElemento>());

        var fotos = (elemento.Galeria.Fotos ?? new List<FotoElemento>())
            .Where(f => !soloActivos || f.EstadoActivo);

        return ((int?)elemento.Galeria.Id, fotos);
    }
}