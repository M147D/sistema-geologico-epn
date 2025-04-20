using Servidor_Sistema_Geologia.Constants;
using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public class Fosil : ElementoGeologico
	{
		[MaxLength(100)]
		public string? Especie { get; set; }

		[MaxLength(100)]
		public string? Periodo { get; set; }

		public SubtipoFosil? TipoFosil { get; set; }
	}
}