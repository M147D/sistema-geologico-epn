using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Services.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Claims;

namespace Servidor_Sistema_Geologia.Controllers;

[Route("api/foto-elementos")]
[ApiController]
public class FotoElementosController : ControllerBase
{
    private readonly IFotoElementoService _fotoService;
    private readonly ILogger<FotoElementosController> _logger;

    public FotoElementosController(
        IFotoElementoService fotoService,
        ILogger<FotoElementosController> logger)
    {
        _fotoService = fotoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las fotos
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FotoElementoDto>>> GetFotos()
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
    public async Task<ActionResult<IEnumerable<FotoElementoDto>>> GetFotosByGaleria(int galeriaId)
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
    /// Obtiene la imagen de una foto específica (con marca de agua para usuarios free)
    /// </summary>
    [HttpGet("imagen/{id}")]
    [Authorize]
    public async Task<ActionResult> GetImagen(int id)
    {
        try
        {
            var imagenBytes = await _fotoService.GetImagenAsync(id);
            if (imagenBytes == null || imagenBytes.Length == 0)
            {
                return NotFound("Imagen no encontrada");
            }

            // Determinar si el usuario es premium o free
            bool isPremium = IsUserPremium();

            byte[] finalImageBytes;

            if (!isPremium)
            {
                // Agregar marca de agua para usuarios free
                finalImageBytes = ApplyWatermark(imagenBytes);
            }
            else
            {
                // No aplicar marca de agua para usuarios premium
                finalImageBytes = imagenBytes;
            }

            return File(finalImageBytes, "image/jpeg");
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
    [Authorize(Roles = "Admin,Premium")]
    public async Task<ActionResult<object>> CreateFoto(int galeriaId, [FromForm] CreateFotoElementoDto fotoDto)
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
    [Authorize(Roles = "Admin,Premium")]
    public async Task<ActionResult<object>> UpdateFoto(int id, [FromForm] UpdateFotoElementoDto fotoDto)
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

    // Helper methods
    private bool TryGetUsuarioId(out int usuarioId)
    {
        usuarioId = 0;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null &&
               int.TryParse(userIdClaim.Value, out usuarioId) &&
               usuarioId > 0;
    }

    private bool IsUserPremium()
    {
        return User.IsInRole("Premium") || User.IsInRole("Admin");
    }

    private byte[] ApplyWatermark(byte[] imageBytes)
    {
        try
        {
            using var inStream = new MemoryStream(imageBytes);
            using var image = Image.FromStream(inStream);
            using var bitmap = new Bitmap(image);
            using var graphics = Graphics.FromImage(bitmap);
            using var outStream = new MemoryStream();

            string watermarkText = "MuseoGeologico";
            ApplyMultipleWatermarks(graphics, bitmap.Width, bitmap.Height, watermarkText);

            bitmap.Save(outStream, ImageFormat.Jpeg);
            return outStream.ToArray();
        }
        catch
        {
            // En caso de error, devolver la imagen original
            return imageBytes;
        }
    }

    private void ApplyMultipleWatermarks(Graphics graphics, int imageWidth, int imageHeight, string text)
    {
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

        using var fontLarge = new Font("Arial", 36, FontStyle.Bold);
        using var fontSmall = new Font("Arial", 16, FontStyle.Bold);
        using var brushMain = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
        using var brushSecondary = new SolidBrush(Color.FromArgb(70, 200, 200, 200));

        // Marca de agua grande en el centro
        var textSizeLarge = graphics.MeasureString(text, fontLarge);
        float xCenter = (imageWidth - textSizeLarge.Width) / 2;
        float yCenter = (imageHeight - textSizeLarge.Height) / 2;
        graphics.DrawString(text, fontLarge, brushMain, xCenter, yCenter);

        // Patrón de marca de agua en toda la imagen
        CreateWatermarkPattern(graphics, imageWidth, imageHeight, text, fontSmall, brushSecondary);
    }

    private void CreateWatermarkPattern(Graphics graphics, int imageWidth, int imageHeight, string text, Font font, Brush brush)
    {
        var textSize = graphics.MeasureString(text, font);
        int spacingX = (int)textSize.Width + 50;
        int spacingY = (int)textSize.Height + 30;

        int columns = (int)Math.Ceiling((double)imageWidth / spacingX) + 1;
        int rows = (int)Math.Ceiling((double)imageHeight / spacingY) + 1;

        for (int row = -1; row < rows; row++)
        {
            int offsetX = (row % 2 == 0) ? 0 : spacingX / 2;

            for (int col = -1; col < columns; col++)
            {
                float x = col * spacingX + offsetX;
                float y = row * spacingY;
                graphics.DrawString(text, font, brush, x, y);
            }
        }
    }
}