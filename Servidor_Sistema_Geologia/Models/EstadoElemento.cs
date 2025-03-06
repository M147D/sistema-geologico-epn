using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia.Models
{
	public class EstadoElemento
	{
		[Key]
		public int Id { get; set; }
		public EstadosElemento DescripcionEstado { get; set; }
	}
}