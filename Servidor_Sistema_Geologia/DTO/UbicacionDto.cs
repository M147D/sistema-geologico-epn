namespace Servidor_Sistema_Geologia.DTO
{
	public class UbicacionDto
	{
		public int Id { get; set; }
		public string? Latitud { get; set; }
		public string? Longitud { get; set; }
		public string? Localidad { get; set; }
		public string? Leyenda { get; set; }

		// Propiedades de navegación
		public PaisDto? Pais { get; set; }
		public ProvinciaDto? Provincia { get; set; }

		// Referencias a IDs para mantener las relaciones
		public int? PaisId { get; set; }
		public int? ProvinciaId { get; set; }
	}
}
