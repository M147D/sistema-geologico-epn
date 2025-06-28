namespace Servidor_Sistema_Geologia;

public class GaleriaElementoGeologicoDto
{
	public int Id { get; set; }
	public string? DetalleGrupo { get; set; }
	public int TotalFotos { get; set; }
	public List<FotoElementoDto> Fotos { get; set; } = new List<FotoElementoDto>();
}

