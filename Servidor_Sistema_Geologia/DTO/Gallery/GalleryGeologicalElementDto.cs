namespace Servidor_Sistema_Geologia.DTO.Gallery;

public class GalleryGeologicalElementDto
{
    public int Id { get; set; }
    public string? DetalleGrupo { get; set; }
    public int TotalFotos { get; set; }
    public List<PhotoElementDto> Fotos { get; set; } = new List<PhotoElementDto>();
}
