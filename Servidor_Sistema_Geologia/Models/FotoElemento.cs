using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public class FotoElemento
    {
		[Key]
		public int Id { get; set; }

		public int? ElementoGeologicoId { get; set; }

		public int? GaleriaElementosGeologicoId { get; set; }

		public byte[]? Imagen { get; set; }

		public string? TipoFoto { get; set; }

		public DateTime FechaSubida { get; set; }

		public string? CreadoPor { get; set; }

		public string? DescripcionEspecifica { get; set; }

		public string? Etiquetas { get; set; }

		public ElementoGeologico? ElementoGeologico { get; set; }

		public GaleriaElementoGeologico? Galeria { get; set; }
	}
}