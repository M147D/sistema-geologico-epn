using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class Provincia
{
	[Key]
	public int Id { get; set; }
	
	public int PaisId { get; set; } = 0;
	
	[MaxLength(100)]
	[Display(Name = "Nombre de la Provincia")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre de la provincia solo puede contener letras y espacios.")]
	[Required(ErrorMessage = "El nombre de la provincia es obligatorio.")]
	public string NombreProvincia { get; set; } = "Desconocido";
	
	// 🔥 CAMPOS PARA SOFT DELETE Y AUDITORÍA
	[Display(Name = "Fecha de Creación")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
	
	[Display(Name = "Estado Activo")]
	public bool EstadoActivo { get; set; } = true;
	
	[Display(Name = "Fecha de Última Actualización")]
	public DateTime? FechaActualizacion { get; set; }
	
	// RELACIONES
	public Pais? Pais { get; set; }
	public List<Ubicacion> Ubicaciones { get; } = new List<Ubicacion>();
}