using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class Pais
{
	[Key]
	public int Id { get; set; }
	
	[MaxLength(100)]
	[Display(Name = "Nombre del País")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre del país solo puede contener letras y espacios.")]
	[Required(ErrorMessage = "El nombre del país es obligatorio.")]
	public string NombrePais { get; set; } = "Desconocido";
	
	// 🔥 CAMPOS PARA SOFT DELETE Y AUDITORÍA
	[Display(Name = "Fecha de Creación")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
	
	[Display(Name = "Estado Activo")]
	public bool EstadoActivo { get; set; } = true;
	
	[Display(Name = "Fecha de Última Actualización")]
	public DateTime? FechaActualizacion { get; set; }
	
	// RELACIONES
	public List<Provincia> Provincias { get; } = new List<Provincia>();
	public List<Ubicacion> Ubicaciones { get; } = new List<Ubicacion>();
}