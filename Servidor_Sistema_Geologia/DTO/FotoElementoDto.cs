namespace Servidor_Sistema_Geologia.DTO
{
	public class FotoElementoDto
	{
		public string? TipoFoto { get; set; }

		public DateTime FechaSubida { get; set; }

		public string? CreadoPor { get; set; }

		public string? DescripcionEspecifica { get; set; }

		public string? Etiquetas { get; set; }

		public byte[]? Imagen { get; set; }

		public ElementoGeologicoDto? ElementoGeologico { get; set; }

		public GaleriaElementoGeologicoDto? Galeria { get; set; }
	}
}
