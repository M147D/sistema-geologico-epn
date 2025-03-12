namespace Servidor_Sistema_Geologia.DTO
{
	public class CreateFotoElementoDto
	{
		public IFormFile? ImagenFile { get; set; }
		public byte[]? Imagen { get; set; }
		public string? TipoFoto { get; set; }
		public string? DescripcionEspecifica { get; set; }
		public string? Etiquetas { get; set; }
	}
}
