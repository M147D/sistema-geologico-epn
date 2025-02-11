using Servidor_Sistema_Geologia.DTO.Create;

namespace Servidor_Sistema_Geologia.DTO
{
	public class FosilCreateDto : ElementoGeologicoCreateDto
	{
		public string? Especie { get; set; }

		public string? Periodo { get; set; }
	}
}
