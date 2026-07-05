using Microsoft.AspNetCore.Http;

namespace Servidor_Sistema_Geologia.DTO.Gallery;

/// <summary>
/// Extends UpdateFotoElementoDto with file upload support for backwards compatibility
/// </summary>
public class UpdatePhotoElementDto : UpdateFotoElementoDto
{
    public IFormFile? ImagenFile { get; set; }
}
