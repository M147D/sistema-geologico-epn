using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public class Ubicacion
	{
		[Key]
		public int Id { get; set; }

		public int? ProvinciaId { get; set; }

		public int? PaisId { get; set; }

		[StringLength(60)]
		public string? Latitud { get; set; }

		[StringLength(60)]
		public string? Longitud { get; set; }

		[StringLength(500)]
		public string? Localidad { get; set; }

		[StringLength(60)]
		public string? Leyenda { get; set; }

		public Pais? Pais { get; set; }

		public Provincia? Provincia { get; set; }

		public List<ElementoGeologico> ElementosGeologicos { get; } = new List<ElementoGeologico>();
	}
}