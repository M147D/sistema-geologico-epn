namespace Servidor_Sistema_Geologia.DTO
{
	public class FiltroElementoDto
	{
		public int? PaisId { get; set; }
		public int? ProvinciaId { get; set; }
		public string? Localidad { get; set; }
		public string? Nombre { get; set; }
		public string? Tipo { get; set; }
	}
}