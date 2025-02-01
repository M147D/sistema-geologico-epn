namespace Servidor_Sistema_Geologia.DTO
{
	public class UbicacionDto
	{
		public int? Id { get; set; }

		public string? Latitud { get; set; }

		public string? Longitud { get; set; }

		public string? Localidad { get; set; }

		public string? Leyenda { get; set; }

		public ProvinciaDto? Provincia { get; set; }

		public PaisDto? Pais { get; set; }
	}
}
