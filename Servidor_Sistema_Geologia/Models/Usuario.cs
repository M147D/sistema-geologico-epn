using Servidor_Sistema_Geologia.Constants;
using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public class Usuario
	{
		[Key]
		public int Id { get; set; }

		public string? NombreCompleto { get; set; }

		public string? NombreUsuario { get; set; }

		public string? Email { get; set; }

		public RolUsuario Rol { get; set; }

		public List<Acceso> Accesos { get; } = new List<Acceso>();
	}
}