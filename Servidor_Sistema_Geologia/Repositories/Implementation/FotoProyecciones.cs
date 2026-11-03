using System.Linq.Expressions;
using Servidor_Sistema_Geologia.Galeria;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

/// <summary>
/// Proyecciones de fotos SIN la columna imagen (bytea). Las listas y los detalles solo
/// necesitan metadatos; los bytes se leen únicamente en GET imagen/{id} (REV-01, H-09).
/// </summary>
internal static class FotoProyecciones
{
    public static readonly Expression<Func<FotoElemento, FotoElemento>> SinImagen = f => new FotoElemento
    {
        Id = f.Id,
        GaleriaElementosGeologicoId = f.GaleriaElementosGeologicoId,
        TipoFoto = f.TipoFoto,
        DescripcionEspecifica = f.DescripcionEspecifica,
        EstadoActivo = f.EstadoActivo,
        FechaCreacion = f.FechaCreacion,
        FechaActualizacion = f.FechaActualizacion,
        FechaEliminacion = f.FechaEliminacion,
        UsuarioCreacionId = f.UsuarioCreacionId,
        UsuarioActualizacionId = f.UsuarioActualizacionId,
        UsuarioEliminacionId = f.UsuarioEliminacionId,
        Imagen = Array.Empty<byte>(),
        Galeria = f.Galeria == null ? null : new GaleriaElementoGeologico { Id = f.Galeria.Id, DetalleGrupo = f.Galeria.DetalleGrupo }
    };
}
