namespace Servidor_Sistema_Geologia.DTO
{
	public class GaleriaElementoGeologicoDto
	{
		public int Id { get; set; }

		public string? DetalleGrupo { get; set; }

		public ElementoGeologicoReadDto? ElementoGeologico { get; set; }

		public List<FotoElementoDto> Fotos { get; set; } = new List<FotoElementoDto>();
	}
}
