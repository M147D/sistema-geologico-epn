using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia.DTO
{
	public class UsuarioDto
	{
		public int Id { get; set; }

		public string? NombreCompleto { get; set; }

		public string? NombreUsuario { get; set; }

		public string? Email { get; set; }

		public RolUsuario Rol { get; set; }
	}
}
