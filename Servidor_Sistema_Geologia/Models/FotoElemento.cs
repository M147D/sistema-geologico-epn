using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class FotoElemento
{
	[Key]
	public int Id { get; set; }
	public int? GaleriaElementosGeologicoId { get; set; }

	public byte[]? Imagen { get; set; }

	[MaxLength(50)]
	public string? TipoFoto { get; set; }

	public DateTime? FechaSubida { get; set; }

	[MaxLength(100)]
	public string? CreadoPor { get; set; }

	[MaxLength(500)]
	public string? DescripcionEspecifica { get; set; }

	[MaxLength(200)]
	public string? Etiquetas { get; set; }

	public GaleriaElementoGeologico? Galeria { get; set; }
}