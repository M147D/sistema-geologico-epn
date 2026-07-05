using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class ProvinciaRepository : IProvinciaRepository
{
    private readonly SistemaGeologicoDbContext _context;

    public ProvinciaRepository(SistemaGeologicoDbContext context)
    {
        _context = context;
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
}
