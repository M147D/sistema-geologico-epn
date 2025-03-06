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

		[MaxLength(100)]
		public string? Nombre { get; set; }

		public ulong? Edad { get; set; }
		[MaxLength(100)]
		public string? Donante { get; set; }

		public DateTime? FechaIngreso { get; set; }
		[MaxLength(50)]
		public string? Codigo { get; set; }

		public uint? Ejemplares { get; set; }
		[MaxLength(500)]
		public string? DocumentosRelacionados { get; set; }
		[MaxLength(200)]
		public string? LaminaURL { get; set; }

		public bool? LaminaExiste { get; set; }

		public Ubicacion? Ubicacion { get; set; }
		public EstadoElemento? EstadoElemento { get; set; }
		public GaleriaElementoGeologico? Galeria { get; set; }
	}
}