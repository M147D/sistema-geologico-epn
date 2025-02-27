namespace Servidor_Sistema_Geologia.DTO
{
	public class CreateFosilDto
	{
		// Propiedades propias del fósil
		public string? Especie { get; set; }
		public string? Periodo { get; set; }
		public string? Nombre { get; set; }
		public int? Edad { get; set; }
		public string? Donante { get; set; }
		public int? FechaIngreso { get; set; }
		public string? Codigo { get; set; }
		public int? Ejemplares { get; set; }
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

		// Estado
		public string? DescripcionEstado { get; set; }

		// Usuario para registro de acceso
		public int UsuarioId { get; set; }
	}
}
