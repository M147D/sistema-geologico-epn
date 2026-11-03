using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

/// <summary>
/// Punto y localidad de un elemento geológico.
/// El país NO se guarda aquí: se obtiene de la provincia (Provincia.Pais), para que
/// no haya dos fuentes del país (REV-01, H-05). Cada país tiene una provincia
/// "Desconocida" para los casos en que no se sabe la provincia.
/// </summary>
public class Ubicacion : EntidadAuditable
{
	public int ProvinciaId { get; set; }

	/// <summary>Grados decimales WGS84, −90..90. Nulo si no se conoce (REV-01, H-06).</summary>
	[Range(-90.0, 90.0, ErrorMessage = "La latitud debe estar entre -90 y 90.")]
	public double? Latitud { get; set; }

	/// <summary>Grados decimales WGS84, −180..180. Nulo si no se conoce.</summary>
	[Range(-180.0, 180.0, ErrorMessage = "La longitud debe estar entre -180 y 180.")]
	public double? Longitud { get; set; }

	[StringLength(500)]
	[RegularExpression(@"^[a-zA-Z0-9\s,.-]+$", ErrorMessage = "La localidad solo puede contener letras, números, espacios y algunos caracteres especiales.")]
	public string Localidad { get; set; } = "Desconocida";

	[StringLength(60)]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre de la ubicación solo puede contener letras y espacios.")]
	public string Leyenda { get; set; } = "Sin leyenda";

	// RELACIONES
	public Provincia? Provincia { get; set; }
	public List<ElementoGeologico> ElementosGeologicos { get; } = new List<ElementoGeologico>();
}
