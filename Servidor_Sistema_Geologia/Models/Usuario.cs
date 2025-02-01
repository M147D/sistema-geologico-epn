using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia.Models
{
	public class Usuario
	{
		[Key]
		public int Id { get; set; }

		public string? Nombres { get; set; }

		public string? Apellidos { get; set; }

		public string? NombreUsuario { get; set; }

		public string? CorreoUsuario { get; set; }

		public RolUsuario Rol { get; set; }

		public List<Acceso> Accesos { get; } = new List<Acceso>();
	}
}