using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Servidor_Sistema_Geologia.DTO.Gallery;
using Servidor_Sistema_Geologia.Services.Interfaces;
using System.Security.Claims;

namespace Servidor_Sistema_Geologia.Controllers;

[Route("api/foto-elementos")]
[ApiController]
public class FotoElementosController : ControllerBase
{
    private readonly IFotoElementoService _fotoService;
    private readonly ILogger<FotoElementosController> _logger;
    private readonly IMemoryCache _cache;
    private readonly IImageProcessingService _imageProcessing;

    public FotoElementosController(
        IFotoElementoService fotoService,
        ILogger<FotoElementosController> logger,
        IMemoryCache cache,
        IImageProcessingService imageProcessing)
    {
        _fotoService = fotoService;
        _logger = logger;
        _cache = cache;
        _imageProcessing = imageProcessing;
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

            if (thumb)
                imagenBytes = _imageProcessing.CreateThumbnail(imagenBytes, 300, 200);

            // Falla cerrado: si la marca no se puede aplicar, se lanza y no se entrega la foto.
            if (!isPremium)
                imagenBytes = _imageProcessing.ApplyWatermark(imagenBytes);

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

            var galeriaId = await _fotoService.GetOrCreateGaleriaIdAsync(elementoId);
            if (galeriaId == null)
                return NotFound($"Elemento con ID {elementoId} no encontrado");

            if (fotoDto.ImagenFile != null && fotoDto.ImagenFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await fotoDto.ImagenFile.CopyToAsync(memoryStream);
                fotoDto.Imagen = memoryStream.ToArray();
            }

            if (fotoDto.Imagen == null || fotoDto.Imagen.Length == 0)
                return BadRequest("Se requiere una imagen");

            var foto = await _fotoService.CreateAsync(fotoDto, galeriaId.Value, usuarioId);

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
            var result = await _fotoService.GetFotosByElementoAsync(elementoId, IsUserAdmin());
            if (result == null)
                return NotFound($"Elemento con ID {elementoId} no encontrado");

            return Ok(result);
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
}
