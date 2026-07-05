using Microsoft.AspNetCore.Http;

namespace Servidor_Sistema_Geologia.DTO.Gallery;

/// <summary>
/// Extends CreateFotoElementoDto with file upload support for backwards compatibility
/// </summary>
public class CreatePhotoElementDto : CreateFotoElementoDto
{
    public IFormFile? ImagenFile { get; set; }
}
