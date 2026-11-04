namespace Servidor_Sistema_Geologia.Services.Interfaces;

/// <summary>
/// Miniaturas y marca de agua de las fotos. Multiplataforma (SkiaSharp).
/// </summary>
public interface IImageProcessingService
{
    /// <summary>
    /// Comprueba que los bytes sean una imagen JPEG, PNG o WebP de tamaño razonable en
    /// píxeles, leyendo solo la cabecera. Devuelve null si es válida o el mensaje de error.
    /// </summary>
    string? ValidarImagen(byte[] imageBytes);

    /// <summary>Reduce la imagen para que quepa en maxWidth × maxHeight, conservando la proporción. Devuelve JPEG.</summary>
    byte[] CreateThumbnail(byte[] imageBytes, int maxWidth, int maxHeight);

    /// <summary>
    /// Aplica la marca de agua del museo. Devuelve JPEG.
    /// Lanza una excepción si no puede aplicarla: nunca devuelve la imagen sin marca.
    /// </summary>
    byte[] ApplyWatermark(byte[] imageBytes);
}
