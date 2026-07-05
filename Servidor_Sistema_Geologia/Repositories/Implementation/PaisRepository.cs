using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class PaisRepository : IPaisRepository
{
    private readonly SistemaGeologicoDbContext _context;

    public PaisRepository(SistemaGeologicoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pais>> GetAllActiveAsync()
    {
        return await _context.Paises
            .Where(p => p.EstadoActivo)
            .OrderBy(p => p.NombrePais)
            .ToListAsync();
    }
}
