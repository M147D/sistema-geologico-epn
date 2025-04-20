using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public class Mineral : ElementoGeologico
	{
		[MaxLength(100)]
		public string? TipoMineral { get; set; }

		[MaxLength(100)]
		public string? Litologia { get; set; }
	}
}
