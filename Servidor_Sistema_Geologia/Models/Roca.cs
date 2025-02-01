using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servidor_Sistema_Geologia.Models
{
	public class Roca : ElementoGeologico
	{
		public string? TipoRoca { get; set; }

		public string? Litologia { get; set; }
	}
}