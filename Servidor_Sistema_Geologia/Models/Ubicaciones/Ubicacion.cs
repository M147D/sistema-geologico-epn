using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class Ubicacion
{
	[Key]
	public int Id { get; set; }
	
	public int? ProvinciaId { get; set; } = 0;
	public int? PaisId { get; set; } = 0;

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
	
	// 🔥 CAMPOS PARA SOFT DELETE Y AUDITORÍA
	[Display(Name = "Fecha de Creación")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
	
	[Display(Name = "Estado Activo")]
	public bool EstadoActivo { get; set; } = true;
	
	[Display(Name = "Fecha de Última Actualización")]
	public DateTime? FechaActualizacion { get; set; }
	
	// RELACIONES
	public Pais Pais { get; set; } = new Pais();
	public Provincia Provincia { get; set; } = new Provincia();
	public List<ElementoGeologico> ElementosGeologicos { get; } = new List<ElementoGeologico>();
}
