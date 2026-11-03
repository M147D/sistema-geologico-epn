using Servidor_Sistema_Geologia.Services.Interfaces;
using SkiaSharp;

namespace Servidor_Sistema_Geologia.Services.Implementation;

/// <summary>
/// Procesamiento de imágenes con SkiaSharp (Linux, Windows y macOS).
/// Reemplaza a System.Drawing, que solo funciona en Windows (REV-01, H-01).
/// </summary>
public sealed class ImageProcessingService : IImageProcessingService, IDisposable
{
    private const string WatermarkText = "Museo Petrográfico Tomas Feininger";
    private const int JpegQuality = 85;

    // Rutas de fuentes probadas en orden. En la imagen de Docker se instala
    // fonts-dejavu-core; en Windows existe Arial.
    private static readonly string[] DefaultFontPaths =
    {
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
        @"C:\Windows\Fonts\arialbd.ttf",
        "/Library/Fonts/Arial Bold.ttf"
    };

    private readonly ILogger<ImageProcessingService> _logger;
    private readonly SKTypeface _typeface;

    public ImageProcessingService(IConfiguration configuration, ILogger<ImageProcessingService> logger)
    {
        _logger = logger;
        var configured = configuration["ImageProcessing:WatermarkFontPath"];
        var candidates = string.IsNullOrWhiteSpace(configured)
            ? DefaultFontPaths
            : new[] { configured }.Concat(DefaultFontPaths).ToArray();

        var path = candidates.FirstOrDefault(File.Exists);
        if (path == null)
        {
            // Sin fuente no se puede dibujar texto: se falla al arrancar y no al servir
            // fotos sin marca de agua.
            throw new InvalidOperationException(
                "No se encontró una fuente para la marca de agua. Configurar ImageProcessing:WatermarkFontPath " +
                $"o instalar una de: {string.Join(", ", DefaultFontPaths)}");
        }

        _typeface = SKTypeface.FromFile(path)
            ?? throw new InvalidOperationException($"No se pudo cargar la fuente {path}");
        _logger.LogInformation("Fuente de la marca de agua: {Path}", path);
    }

    public byte[] CreateThumbnail(byte[] imageBytes, int maxWidth, int maxHeight)
    {
        using var original = Decode(imageBytes);

        float ratio = Math.Min((float)maxWidth / original.Width, (float)maxHeight / original.Height);
        if (ratio >= 1f)
            return Encode(original);

        int width = Math.Max(1, (int)(original.Width * ratio));
        int height = Math.Max(1, (int)(original.Height * ratio));

        using var resized = original.Resize(
            new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul),
            new SKSamplingOptions(SKCubicResampler.Mitchell))
            ?? throw new InvalidOperationException("No se pudo redimensionar la imagen");
        return Encode(resized);
    }

    public byte[] ApplyWatermark(byte[] imageBytes)
    {
        using var original = Decode(imageBytes);
        var info = new SKImageInfo(original.Width, original.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info)
            ?? throw new InvalidOperationException("No se pudo crear la superficie de dibujo");
        var canvas = surface.Canvas;
        using (var originalImage = SKImage.FromBitmap(original))
            canvas.DrawImage(originalImage, 0, 0, new SKSamplingOptions(SKFilterMode.Linear));

        // Marca grande centrada, con sombra desplazada 2 px para contraste.
        // 72 px como máximo; más chica si no cabe en el 90 % del ancho.
        using var fontLarge = new SKFont(_typeface, 72f);
        float naturalWidth = fontLarge.MeasureText(WatermarkText);
        if (naturalWidth > info.Width * 0.9f)
            fontLarge.Size = 72f * info.Width * 0.9f / naturalWidth;
        using var white = new SKPaint { Color = new SKColor(255, 255, 255, 180), IsAntialias = true };
        using var shadow = new SKPaint { Color = new SKColor(0, 0, 0, 130), IsAntialias = true };
        float largeWidth = fontLarge.MeasureText(WatermarkText);
        float x = (info.Width - largeWidth) / 2f;
        float y = info.Height / 2f - (fontLarge.Metrics.Ascent + fontLarge.Metrics.Descent) / 2f;
        canvas.DrawText(WatermarkText, x + 2, y + 2, SKTextAlign.Left, fontLarge, shadow);
        canvas.DrawText(WatermarkText, x, y, SKTextAlign.Left, fontLarge, white);

        // Patrón repetido, rotado -30° alrededor del centro.
        using var fontPattern = new SKFont(_typeface, 32f);
        using var patternPaint = new SKPaint { Color = new SKColor(255, 255, 255, 110), IsAntialias = true };
        float textWidth = fontPattern.MeasureText(WatermarkText);
        float textHeight = fontPattern.Metrics.Descent - fontPattern.Metrics.Ascent;
        float spacingX = textWidth + 60;
        float spacingY = textHeight + 50;

        canvas.Save();
        canvas.Translate(info.Width / 2f, info.Height / 2f);
        canvas.RotateDegrees(-30);
        canvas.Translate(-info.Width / 2f, -info.Height / 2f);

        int cols = (int)Math.Ceiling(info.Width * 1.5f / spacingX) + 2;
        int rows = (int)Math.Ceiling(info.Height * 1.5f / spacingY) + 2;
        float startX = -info.Width * 0.25f;
        float startY = -info.Height * 0.25f - fontPattern.Metrics.Ascent;

        for (int row = 0; row < rows; row++)
        {
            float offsetX = row % 2 == 0 ? 0 : spacingX / 2;
            for (int col = 0; col < cols; col++)
            {
                canvas.DrawText(WatermarkText, startX + col * spacingX + offsetX, startY + row * spacingY,
                    SKTextAlign.Left, fontPattern, patternPaint);
            }
        }
        canvas.Restore();

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, JpegQuality)
            ?? throw new InvalidOperationException("No se pudo codificar la imagen con marca de agua");
        return data.ToArray();
    }

    private static SKBitmap Decode(byte[] imageBytes) =>
        SKBitmap.Decode(imageBytes) ?? throw new InvalidDataException("El archivo no es una imagen válida");

    private static byte[] Encode(SKBitmap bitmap)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, JpegQuality)
            ?? throw new InvalidOperationException("No se pudo codificar la imagen");
        return data.ToArray();
    }

    public void Dispose() => _typeface.Dispose();
}
