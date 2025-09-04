namespace Servidor_Sistema_Geologia.DTO;

public class FotoElementoDto
{
    public int Id { get; set; }
    public int GaleriaElementosGeologicoId { get; set; }
    public TipoFoto TipoFoto { get; set; }
    public string DescripcionEspecifica { get; set; } = "Vacío";
    public string ImagenUrl { get; set; } = string.Empty;
    
    // Información de la galería asociada
    public string? DetalleGrupoGaleria { get; set; }
    public string? NombreElementoGeologico { get; set; }
}

public class CreateFotoElementoDto
{
    public TipoFoto TipoFoto { get; set; } = TipoFoto.Desconocido;
    public string DescripcionEspecifica { get; set; } = "Vacío";
    public IFormFile? ImagenFile { get; set; }
    public byte[]? Imagen { get; set; }
}

public class UpdateFotoElementoDto
{
    public TipoFoto TipoFoto { get; set; }
    public string DescripcionEspecifica { get; set; } = "Vacío";
    public IFormFile? ImagenFile { get; set; }
    public byte[]? Imagen { get; set; }
}

