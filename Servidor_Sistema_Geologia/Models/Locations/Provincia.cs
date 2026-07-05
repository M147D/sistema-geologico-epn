using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class Provincia : EntidadAuditable
{
	public int PaisId { get; set; }

	[MaxLength(100)]
	[Display(Name = "Nombre de la Provincia")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre de la provincia solo puede contener letras y espacios.")]
	[Required(ErrorMessage = "El nombre de la provincia es obligatorio.")]
	public string NombreProvincia { get; set; } = "Desconocido";

	// RELACIONES
	public Pais? Pais { get; set; }
	public List<Ubicacion> Ubicaciones { get; } = new List<Ubicacion>();
}