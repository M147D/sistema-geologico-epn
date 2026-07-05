using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class Ubicacion : EntidadAuditable
{
	// FOREIGN KEYS - Con valores por defecto apuntando a registros "Desconocido"
	public int PaisId { get; set; } = 1; // País "Desconocido" por defecto
	public int ProvinciaId { get; set; } = 1; // Provincia "Desconocida" por defecto

	[StringLength(60)]
	[RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "La latitud debe ser un número válido.")]
	public string Latitud { get; set; } = "0.0";

	[StringLength(60)]
	[RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "La longitud debe ser un número válido.")]
	public string Longitud { get; set; } = "0.0";

	[StringLength(500)]
	[RegularExpression(@"^[a-zA-Z0-9\s,.-]+$", ErrorMessage = "La localidad solo puede contener letras, números, espacios y algunos caracteres especiales.")]
	public string Localidad { get; set; } = "Desconocida";

	[StringLength(60)]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre de la ubicación solo puede contener letras y espacios.")]
	public string Leyenda { get; set; } = "Sin leyenda";

	// RELACIONES
	public Pais? Pais { get; set; } // Relación directa con País
	public Provincia? Provincia { get; set; } // Relación con Provincia
	public List<ElementoGeologico> ElementosGeologicos { get; } = new List<ElementoGeologico>();
}
