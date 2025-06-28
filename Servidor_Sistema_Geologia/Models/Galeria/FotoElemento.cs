using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Galeria;

public class FotoElemento
{
	[Key]
	public int Id { get; set; }
	public int GaleriaElementosGeologicoId { get; set; }
	public byte[] Imagen { get; set; } = [0];
	public TipoFoto TipoFoto { get; set; } = TipoFoto.Desconocido;
	[MaxLength(500)]
	[Display(Name = "Descripción")]
	[RegularExpression(@"^[a-zA-Z0-9\s,.-]+$", ErrorMessage = "La descripción solo puede contener letras, números, espacios y algunos caracteres especiales.")]
	public string DescripcionEspecifica { get; set; } = "Vacío";
	public GaleriaElementoGeologico Galeria { get; set; } = new GaleriaElementoGeologico();
}