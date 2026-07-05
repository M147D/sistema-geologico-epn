using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class Pais : EntidadAuditable
{
	[MaxLength(100)]
	[Display(Name = "Nombre del País")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre del país solo puede contener letras y espacios.")]
	[Required(ErrorMessage = "El nombre del país es obligatorio.")]
	public string NombrePais { get; set; } = "Desconocido";

	// RELACIONES
	public List<Provincia> Provincias { get; } = new List<Provincia>();
}