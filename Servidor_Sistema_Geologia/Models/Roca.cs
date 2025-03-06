using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public class Roca : ElementoGeologico
	{
		[MaxLength(100)]
		public string? TipoRoca { get; set; }

		[MaxLength(100)]
		public string? Litologia { get; set; }
	}
}