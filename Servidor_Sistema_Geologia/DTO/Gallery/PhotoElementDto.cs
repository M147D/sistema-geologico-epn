namespace Servidor_Sistema_Geologia.DTO.Gallery;

public class PhotoElementDto
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
