using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

/// <summary>
/// Países, provincias y ubicaciones. Las entidades nuevas se agregan al contexto y se
/// guardan junto con el elemento en un solo SaveChanges (REV-01, H-08).
/// </summary>
public class UbicacionRepository : IUbicacionRepository
{
    public const string PaisDesconocido = "Desconocido";
    public const string ProvinciaDesconocida = "Desconocida";

    private readonly SistemaGeologicoDbContext _context;

    public UbicacionRepository(SistemaGeologicoDbContext context) => _context = context;

    public async Task<Pais> ObtenerOCrearPaisAsync(string nombrePais)
    {
        var nombre = nombrePais.Trim();
        var pais = _context.Paises.Local.FirstOrDefault(p => p.NombrePais == nombre && p.EstadoActivo)
            ?? await _context.Paises.FirstOrDefaultAsync(p => p.NombrePais == nombre && p.EstadoActivo);
        if (pais != null) return pais;

        pais = new Pais { NombrePais = nombre, EstadoActivo = true, FechaCreacion = DateTime.Now };
        _context.Paises.Add(pais);
        return pais;
    }

    public async Task<Provincia> ObtenerOCrearProvinciaAsync(string nombreProvincia, Pais pais)
    {
        var nombre = nombreProvincia.Trim();
        var provincia = _context.Provincias.Local.FirstOrDefault(p => p.NombreProvincia == nombre && p.EstadoActivo
                            && (p.Pais == pais || (pais.Id != 0 && p.PaisId == pais.Id)));
        if (provincia == null && pais.Id != 0)
            provincia = await _context.Provincias.FirstOrDefaultAsync(p => p.NombreProvincia == nombre && p.PaisId == pais.Id && p.EstadoActivo);
        if (provincia != null) return provincia;

        provincia = new Provincia { NombreProvincia = nombre, Pais = pais, EstadoActivo = true, FechaCreacion = DateTime.Now };
        _context.Provincias.Add(provincia);
        return provincia;
    }

    public Task<Provincia> ObtenerProvinciaDesconocidaAsync(Pais pais) =>
        ObtenerOCrearProvinciaAsync(ProvinciaDesconocida, pais);

    public async Task<Provincia> ObtenerProvinciaDesconocidaGlobalAsync() =>
        await ObtenerProvinciaDesconocidaAsync(await ObtenerOCrearPaisAsync(PaisDesconocido));

    public async Task<Ubicacion?> GetByIdAsync(int id) =>
        await _context.Ubicaciones.FirstOrDefaultAsync(u => u.Id == id && u.EstadoActivo);
}
