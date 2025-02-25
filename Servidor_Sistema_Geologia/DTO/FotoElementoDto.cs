namespace Servidor_Sistema_Geologia.DTO
{
	public class FotoElementoDto
	{
		public int Id { get; set; }
		public byte[]? Imagen { get; set; }
		public string? TipoFoto { get; set; }
		public DateTime? FechaSubida { get; set; }
		public string? CreadoPor { get; set; }
		public string? DescripcionEspecifica { get; set; }
		public string? Etiquetas { get; set; }
		public GaleriaElementoGeologicoDto? Galeria { get; set; }
	}
}
