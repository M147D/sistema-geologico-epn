using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Galeria;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class FotoElementoRepository : IFotoElementoRepository
{
    private readonly GestorSistemaGeologia _context;

    public FotoElementoRepository(GestorSistemaGeologia context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FotoElemento>> GetAllAsync()
    {
        return await _context.FotosElementos
            .Include(f => f.Galeria)
            .ThenInclude(g => g.ElementoGeologico)
            .ToListAsync();
    }

    public async Task<FotoElemento?> GetByIdAsync(int id)
    {
        return await _context.FotosElementos
            .Include(f => f.Galeria)
            .ThenInclude(g => g.ElementoGeologico)
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
            _context.FotosElementos.Remove(foto);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.FotosElementos.AnyAsync(f => f.Id == id);
    }
}