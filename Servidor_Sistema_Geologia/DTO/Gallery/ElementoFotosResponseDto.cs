namespace Servidor_Sistema_Geologia.DTO.Gallery;

public class ElementoFotosResponseDto
{
    public int? GaleriaId { get; set; }
    public List<FotoElementoDto> Fotos { get; set; } = new();
}
