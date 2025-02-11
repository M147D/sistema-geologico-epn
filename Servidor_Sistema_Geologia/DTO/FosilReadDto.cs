namespace Servidor_Sistema_Geologia.DTO
{
	public class FosilReadDto : ElementoGeologicoReadDto
	{
		public string? Especie { get; set; }

		public string? Periodo { get; set; }
	}
}
