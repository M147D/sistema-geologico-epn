using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia.DTO
{
	public class FosilDto : ElementoGeologicoDto
	{
		public string? Especie { get; set; }
		public string? Periodo { get; set; }
		public SubtipoFosil? TipoFosil { get; set; }
	}
}
