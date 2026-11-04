using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Galeria;

public class FotoElemento : EntidadAuditable
{
	public int GaleriaElementosGeologicoId { get; set; }
	public byte[] Imagen { get; set; } = [0];
	public TipoFoto TipoFoto { get; set; } = TipoFoto.Desconocido;
	[MaxLength(500)]
	[Display(Name = "Descripción")]
	[RegularExpression(@"^[a-zA-Z0-9\s,.-]+$", ErrorMessage = "La descripción solo puede contener letras, números, espacios y algunos caracteres especiales.")]
	public string DescripcionEspecifica { get; set; } = "Vacío";
	public GaleriaElementoGeologico? Galeria { get; set; }

	/// <summary>
	/// Token de concurrencia optimista: la columna de sistema xmin de PostgreSQL. Una
	/// escritura basada en una lectura vieja falla en lugar de pisar otra (PEN-12).
	/// </summary>
	[System.ComponentModel.DataAnnotations.Timestamp]
	public uint Version { get; set; }
}
