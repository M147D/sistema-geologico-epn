using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public class Pais
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(100)]
		public string? NombrePais { get; set; }

		public List<Provincia> Provincias { get; } = new List<Provincia>();

		public List<Ubicacion> Ubicaciones { get; } = new List<Ubicacion>();
	}
}