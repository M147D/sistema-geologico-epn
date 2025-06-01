namespace Servidor_Sistema_Geologia.DTO
{
	public abstract class ElementoGeologicoDto
	{
		public int Id { get; set; }
		public string? Nombre { get; set; }
		public string? Edad { get; set; }
		public string? Donante { get; set; }
		public DateTime? FechaIngreso { get; set; }
		public string? Codigo { get; set; }
		public int? Ejemplares { get; set; }
		public string? DocumentosRelacionados { get; set; }
		public string? LaminaURL { get; set; }
		public bool? LaminaExiste { get; set; }

		// Propiedades de navegación
		public UbicacionDto? Ubicacion { get; set; }
		public GaleriaElementoGeologicoDto? Galeria { get; set; }

		// ID para crear el registro de acceso
		public int UsuarioId { get; set; }

		// Referencias a IDs para mantener las relaciones
		public int? UbicacionId { get; set; }
		public int? EstadoElementoId { get; set; }
		public int? GaleriaElementosGeologicoId { get; set; }
	}
}
