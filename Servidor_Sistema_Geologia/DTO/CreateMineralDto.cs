namespace Servidor_Sistema_Geologia.DTO
{
	public class CreateMineralDto
	{
		// Propiedades específicas del mineral
		public string? TipoMineral { get; set; }
		public string? Litologia { get; set; }

		// Propiedades heredadas de ElementoGeologico
		public string? Nombre { get; set; }
		public ulong? Edad { get; set; }
		public string? Donante { get; set; }
		public string? Codigo { get; set; }
		public uint? Ejemplares { get; set; }
		public string? DocumentosRelacionados { get; set; }
		public string? LaminaURL { get; set; }
		public bool? LaminaExiste { get; set; }

		// Datos de ubicación
		public string? Latitud { get; set; }
		public string? Longitud { get; set; }
		public string? Localidad { get; set; }
		public string? Leyenda { get; set; }
		public string? NombreProvincia { get; set; }
		public string? NombrePais { get; set; }

		// Usuario para registro de acceso
		public int UsuarioId { get; set; }
	}
}