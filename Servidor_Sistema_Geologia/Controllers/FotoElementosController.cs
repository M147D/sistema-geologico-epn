using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Servidor_Sistema_Geologia.DTO.Gallery;
using Servidor_Sistema_Geologia.Galeria;
using Servidor_Sistema_Geologia.Services.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using System.Security.Claims;

namespace Servidor_Sistema_Geologia.Controllers;

[Route("api/foto-elementos")]
[ApiController]
public class FotoElementosController : ControllerBase
{
    private readonly IFotoElementoService _fotoService;
    private readonly ILogger<FotoElementosController> _logger;
    private readonly SistemaGeologicoDbContext _context;
    private readonly IMemoryCache _cache;

    public FotoElementosController(
        IFotoElementoService fotoService,
        ILogger<FotoElementosController> logger,
        SistemaGeologicoDbContext context,
        IMemoryCache cache)
    {
        _fotoService = fotoService;
        _logger = logger;
        _context = context;
        _cache = cache;
    }

    /// <summary>
    /// Obtiene todas las fotos
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PhotoElementDto>>> GetFotos()
    {
        try
        {
            var fotos = await _fotoService.GetAllAsync();
            return Ok(fotos);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("No se encontraron fotos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las fotos");
            return BadRequest($"Error al obtener las fotos: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene las fotos de una galería específica
    /// </summary>
    [HttpGet("galeria/{galeriaId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PhotoElementDto>>> GetFotosByGaleria(int galeriaId)
    {
        try
        {
            var fotos = await _fotoService.GetByGaleriaIdAsync(galeriaId);
            return Ok(fotos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener fotos de galería {GaleriaId}", galeriaId);
            return BadRequest($"Error al obtener las fotos: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene la imagen de una foto específica (con marca de agua para usuarios free).
    /// Acepta ?thumb=true para devolver una miniatura 300×200.
    /// Resultados cacheados en IMemoryCache para evitar re-procesar GDI+.
    /// </summary>
    [HttpGet("imagen/{id}")]
    [Authorize]
    public async Task<ActionResult> GetImagen(int id, [FromQuery] bool thumb = false)
    {
        try
        {
            bool isPremium = IsUserPremium();
            string cacheKey = $"foto_{id}_{(isPremium ? "clean" : "wm")}_{(thumb ? "thumb" : "full")}";

            if (_cache.TryGetValue(cacheKey, out byte[]? cached))
                return File(cached!, "image/jpeg");

            var imagenBytes = await _fotoService.GetImagenAsync(id);
            if (imagenBytes == null || imagenBytes.Length == 0)
                return NotFound("Imagen no encontrada");

            if (thumb && OperatingSystem.IsWindows())
                imagenBytes = ResizeImage(imagenBytes, 300, 200);

            if (!isPremium && OperatingSystem.IsWindows())
                imagenBytes = ApplyWatermark(imagenBytes);

            _cache.Set(cacheKey, imagenBytes, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                Size = imagenBytes.Length
            });

            return File(imagenBytes, "image/jpeg");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Imagen no encontrada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener imagen {Id}", id);
            return BadRequest($"Error al obtener la imagen: {ex.Message}");
        }
    }

    /// <summary>
    /// Crea una nueva foto en una galería específica
    /// </summary>
    [HttpPost("galeria/{galeriaId}")]
    [Authorize(Roles = "Admin,Invitado")]
    public async Task<ActionResult<object>> CreateFoto(int galeriaId, [FromForm] CreatePhotoElementDto fotoDto)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            // Procesar el archivo de imagen y convertirlo a byte[]
            if (fotoDto.ImagenFile != null && fotoDto.ImagenFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await fotoDto.ImagenFile.CopyToAsync(memoryStream);
                fotoDto.Imagen = memoryStream.ToArray();
            }

            if (fotoDto.Imagen == null || fotoDto.Imagen.Length == 0)
            {
                return BadRequest("Se requiere una imagen");
            }

            var foto = await _fotoService.CreateAsync(fotoDto, galeriaId, usuarioId);

            // Crear respuesta simplificada sin incluir la imagen
            var respuesta = new
            {
                foto.Id,
                foto.GaleriaElementosGeologicoId,
                foto.TipoFoto,
                foto.DescripcionEspecifica,
                foto.ImagenUrl,
                foto.DetalleGrupoGaleria,
                foto.NombreElementoGeologico
            };

            return CreatedAtAction(nameof(GetImagen), new { id = foto.Id }, respuesta);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear foto en galería {GaleriaId}", galeriaId);
            return BadRequest($"Error al crear la foto: {ex.Message}");
        }
    }

    /// <summary>
    /// Actualiza una foto existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Invitado")]
    public async Task<ActionResult<object>> UpdateFoto(int id, [FromForm] UpdatePhotoElementDto fotoDto)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            // Procesar el archivo de imagen si se proporciona
            if (fotoDto.ImagenFile != null && fotoDto.ImagenFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await fotoDto.ImagenFile.CopyToAsync(memoryStream);
                fotoDto.Imagen = memoryStream.ToArray();
            }

            var foto = await _fotoService.UpdateAsync(id, fotoDto, usuarioId);

            // Crear respuesta simplificada
            var respuesta = new
            {
                foto.Id,
                foto.GaleriaElementosGeologicoId,
                foto.TipoFoto,
                foto.DescripcionEspecifica,
                foto.ImagenUrl,
                foto.DetalleGrupoGaleria,
                foto.NombreElementoGeologico
            };

            InvalidateFotoCache(id);
            return Ok(respuesta);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar foto {Id}", id);
            return BadRequest($"Error al actualizar la foto: {ex.Message}");
        }
    }

    /// <summary>
    /// Elimina una foto
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteFoto(int id)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            await _fotoService.DeleteAsync(id, usuarioId);
            InvalidateFotoCache(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar foto {Id}", id);
            return BadRequest($"Error al eliminar la foto: {ex.Message}");
        }
    }

    /// <summary>
    /// Sube una foto a un elemento geológico por su ID.
    /// Crea la galería automáticamente si el elemento no tiene una.
    /// </summary>
    [HttpPost("elemento/{elementoId}")]
    [Authorize(Roles = "Admin,Invitado")]
    public async Task<ActionResult<object>> CreateFotoByElemento(int elementoId, [FromForm] CreatePhotoElementDto fotoDto)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
                return Unauthorized("Usuario no autenticado");

            var elemento = await _context.ElementosGeologicos
                .Include(e => e.Galeria)
                .FirstOrDefaultAsync(e => e.Id == elementoId && e.EstadoActivo);

            if (elemento == null)
                return NotFound($"Elemento con ID {elementoId} no encontrado");

            // Crear galería si no existe
            if (elemento.Galeria == null)
            {
                var nuevaGaleria = new GaleriaElementoGeologico
                {
                    ElementoGeologicoId = elementoId,
                    DetalleGrupo = "Galeria",
                    FechaCreacion = DateTime.Now,
                    EstadoActivo = true
                };
                _context.GaleriaElementosGeologicos.Add(nuevaGaleria);
                await _context.SaveChangesAsync();
                elemento.Galeria = nuevaGaleria;
            }

            if (fotoDto.ImagenFile != null && fotoDto.ImagenFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await fotoDto.ImagenFile.CopyToAsync(memoryStream);
                fotoDto.Imagen = memoryStream.ToArray();
            }

            if (fotoDto.Imagen == null || fotoDto.Imagen.Length == 0)
                return BadRequest("Se requiere una imagen");

            var foto = await _fotoService.CreateAsync(fotoDto, elemento.Galeria.Id, usuarioId);

            var respuesta = new
            {
                foto.Id,
                foto.GaleriaElementosGeologicoId,
                foto.TipoFoto,
                foto.DescripcionEspecifica,
                foto.ImagenUrl,
                foto.NombreElementoGeologico
            };

            return CreatedAtAction(nameof(GetImagen), new { id = foto.Id }, respuesta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear foto para elemento {ElementoId}", elementoId);
            return BadRequest($"Error al crear la foto: {ex.Message}");
        }
    }

    /// <summary>
    /// Restaura una foto eliminada (soft-delete → activo). Solo Admin.
    /// </summary>
    [HttpPatch("{id}/restaurar")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RestoreFoto(int id)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
                return Unauthorized("Usuario no autenticado");

            await _fotoService.RestoreAsync(id, usuarioId);
            InvalidateFotoCache(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restaurar foto {Id}", id);
            return BadRequest($"Error al restaurar la foto: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene la galería y metadatos de fotos de un elemento geológico (sin bytes de imagen)
    /// </summary>
    [HttpGet("por-elemento/{elementoId}")]
    [Authorize]
    public async Task<ActionResult> GetFotosByElemento(int elementoId)
    {
        try
        {
            var elemento = await _context.ElementosGeologicos
                .Include(e => e.Galeria)
                    .ThenInclude(g => g!.Fotos)
                .FirstOrDefaultAsync(e => e.Id == elementoId && e.EstadoActivo);

            if (elemento == null)
                return NotFound($"Elemento con ID {elementoId} no encontrado");

            if (elemento.Galeria == null)
                return Ok(new { galeriaId = (int?)null, fotos = new List<object>() });

            bool adminVista = IsUserAdmin();

            var fotos = (elemento.Galeria.Fotos ?? new List<FotoElemento>())
                .Where(f => adminVista || f.EstadoActivo)
                .Select(f => new
                {
                    id = f.Id,
                    galeriaElementosGeologicoId = f.GaleriaElementosGeologicoId,
                    tipoFoto = f.TipoFoto,
                    descripcionEspecifica = f.DescripcionEspecifica,
                    fechaCreacion = f.FechaCreacion,
                    fechaActualizacion = f.FechaActualizacion,
                    estadoActivo = f.EstadoActivo,
                    imagenUrl = $"/api/foto-elementos/imagen/{f.Id}"
                }).ToList();

            return Ok(new { galeriaId = (int?)elemento.Galeria.Id, fotos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener fotos del elemento {ElementoId}", elementoId);
            return BadRequest($"Error al obtener las fotos: {ex.Message}");
        }
    }

    // Helper methods
    private bool TryGetUsuarioId(out int usuarioId)
    {
        usuarioId = 0;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null &&
               int.TryParse(userIdClaim.Value, out usuarioId) &&
               usuarioId > 0;
    }

    private bool IsUserAdmin()
    {
        return User.IsInRole("Admin");
    }

    private bool IsUserPremium()
    {
        return User.IsInRole("Premium") || User.IsInRole("Admin") || User.IsInRole("Invitado");
    }

    private void InvalidateFotoCache(int id)
    {
        _cache.Remove($"foto_{id}_clean_full");
        _cache.Remove($"foto_{id}_wm_full");
        _cache.Remove($"foto_{id}_clean_thumb");
        _cache.Remove($"foto_{id}_wm_thumb");
    }

    private const string WatermarkText = "Museo Petrográfico Tomas Feininger";

    [SupportedOSPlatform("windows")]
    private byte[] ResizeImage(byte[] imageBytes, int maxWidth, int maxHeight)
    {
        try
        {
            using var inStream = new MemoryStream(imageBytes);
            using var original = Image.FromStream(inStream);

            float ratioX = (float)maxWidth / original.Width;
            float ratioY = (float)maxHeight / original.Height;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = Math.Max(1, (int)(original.Width * ratio));
            int newHeight = Math.Max(1, (int)(original.Height * ratio));

            using var bitmap = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);

            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            graphics.DrawImage(original, 0, 0, newWidth, newHeight);

            using var outStream = new MemoryStream();
            bitmap.Save(outStream, ImageFormat.Jpeg);
            return outStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al redimensionar imagen {Width}x{Height}", maxWidth, maxHeight);
            return imageBytes;
        }
    }

    [SupportedOSPlatform("windows")]
    private byte[] ApplyWatermark(byte[] imageBytes)
    {
        try
        {
            using var inStream = new MemoryStream(imageBytes);
            using var original = Image.FromStream(inStream);

            // Format32bppArgb garantiza soporte para transparencia y que Graphics.FromImage no falle
            using var bitmap = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            // Dibujar imagen original primero
            graphics.DrawImage(original, 0, 0, original.Width, original.Height);

            // Aplicar marca de agua
            ApplyMultipleWatermarks(graphics, bitmap.Width, bitmap.Height);

            using var outStream = new MemoryStream();
            bitmap.Save(outStream, ImageFormat.Jpeg);
            return outStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aplicar marca de agua");
            return imageBytes;
        }
    }

    [SupportedOSPlatform("windows")]
    private void ApplyMultipleWatermarks(Graphics graphics, int imageWidth, int imageHeight)
    {
        using var fontLarge = new Font("Arial", 54, FontStyle.Bold);
        using var fontPattern = new Font("Arial", 24, FontStyle.Bold);

        // Texto blanco con sombra oscura para visibilidad sobre cualquier fondo
        using var brushWhite = new SolidBrush(Color.FromArgb(180, 255, 255, 255));
        using var brushShadow = new SolidBrush(Color.FromArgb(130, 0, 0, 0));
        using var brushPattern = new SolidBrush(Color.FromArgb(110, 255, 255, 255));

        // Marca grande centrada
        var textSize = graphics.MeasureString(WatermarkText, fontLarge);
        float xCenter = (imageWidth - textSize.Width) / 2;
        float yCenter = (imageHeight - textSize.Height) / 2;

        // Sombra (desplazada 2px) para contraste
        graphics.DrawString(WatermarkText, fontLarge, brushShadow, xCenter + 2, yCenter + 2);
        // Texto principal blanco
        graphics.DrawString(WatermarkText, fontLarge, brushWhite, xCenter, yCenter);

        // Patrón repetido rotado
        CreateWatermarkPattern(graphics, imageWidth, imageHeight, fontPattern, brushPattern);
    }

    [SupportedOSPlatform("windows")]
    private void CreateWatermarkPattern(Graphics graphics, int imageWidth, int imageHeight, Font font, Brush brush)
    {
        var textSize = graphics.MeasureString(WatermarkText, font);
        float spacingX = textSize.Width + 60;
        float spacingY = textSize.Height + 50;

        var state = graphics.Save();
        try
        {
            // Rotar el patrón -30° alrededor del centro de la imagen
            graphics.TranslateTransform(imageWidth / 2f, imageHeight / 2f);
            graphics.RotateTransform(-30);
            graphics.TranslateTransform(-imageWidth / 2f, -imageHeight / 2f);

            int cols = (int)Math.Ceiling(imageWidth * 1.5f / spacingX) + 2;
            int rows = (int)Math.Ceiling(imageHeight * 1.5f / spacingY) + 2;
            float startX = -imageWidth * 0.25f;
            float startY = -imageHeight * 0.25f;

            for (int row = 0; row < rows; row++)
            {
                float offsetX = (row % 2 == 0) ? 0 : spacingX / 2;
                for (int col = 0; col < cols; col++)
                {
                    graphics.DrawString(WatermarkText, font, brush,
                        startX + col * spacingX + offsetX,
                        startY + row * spacingY);
                }
            }
        }
        finally
        {
            graphics.Restore(state);
        }
    }
}
