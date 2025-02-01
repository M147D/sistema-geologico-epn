using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public class Provincia
	{
		[Key]
		public int Id { get; set; }

		public int? PaisId { get; set; }

		[MaxLength(100)]
		public string? NombreProvincia { get; set; }

		public Pais? Pais { get; set; }

		public List<Ubicacion> Ubicaciones { get; } = new List<Ubicacion>();
	}
}