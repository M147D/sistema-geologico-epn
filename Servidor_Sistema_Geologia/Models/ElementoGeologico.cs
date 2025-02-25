using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public abstract class ElementoGeologico
    {
		[Key]
		public int Id { get; set; }

		public int? EstadoElementoId { get; set; }

		public int? UbicacionId { get; set; }

		public int? GaleriaElementosGeologicoId { get; set; }

		public string? Nombre { get; set; }

		public int? Edad { get; set; }

		public string? Donante { get; set; }

		public int? FechaIngreso { get; set; }

		public string? Codigo { get; set; }

		public int? Ejemplares { get; set; }

		public string? DocumentosRelacionados { get; set; }

		public string? LaminaURL { get; set; }

		public bool? LaminaExiste { get; set; }

		public Ubicacion? Ubicacion { get; set; }

		public EstadoElemento? EstadoElemento { get; set; }

		public GaleriaElementoGeologico? Galeria { get; set; }
	}
}