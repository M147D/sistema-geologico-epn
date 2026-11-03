namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

/// <summary>
/// Acceso a datos de países, provincias y ubicaciones usado por el alta de elementos.
/// Los servicios no tocan el DbContext (REV-01, P-02).
/// </summary>
public interface IUbicacionRepository
{
    /// <summary>País activo por nombre, o uno nuevo agregado al contexto (sin guardar).</summary>
    Task<Pais> ObtenerOCrearPaisAsync(string nombrePais);

    /// <summary>Provincia activa por nombre dentro del país, o una nueva agregada al contexto (sin guardar).</summary>
    Task<Provincia> ObtenerOCrearProvinciaAsync(string nombreProvincia, Pais pais);

    /// <summary>La provincia "Desconocida" del país (la crea si falta; sin guardar).</summary>
    Task<Provincia> ObtenerProvinciaDesconocidaAsync(Pais pais);

    /// <summary>País "Desconocido" con su provincia "Desconocida" (los crea si faltan; sin guardar).</summary>
    Task<Provincia> ObtenerProvinciaDesconocidaGlobalAsync();

    /// <summary>Ubicación activa por id.</summary>
    Task<Ubicacion?> GetByIdAsync(int id);
}
