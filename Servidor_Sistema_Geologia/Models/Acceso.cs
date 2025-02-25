using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia.Models
{
	public class Acceso
	{
		[Key]
		public int Id { get; set; }

		public int? UsuarioId { get; set; }

		public int? ElementoGeologicoId { get; set; }

		public DateTime? FechaAcceso { get; set; }

		public AccionesUsuario? Accion { get; set; }

		public Usuario? Usuario { get; set; }

		public ElementoGeologico? ElementoGeologico { get; set; }
	}
}