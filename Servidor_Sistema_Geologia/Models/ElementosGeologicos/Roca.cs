using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class Roca : ElementoGeologico
{
	[EnumDataType(typeof(SubtipoRoca), ErrorMessage = "El tipo de roca no es válido.")]
	[Display(Name = "Tipo de Roca")]
	public SubtipoRoca TipoRoca { get; set; } = SubtipoRoca.Desconocido;
	[MaxLength(500)]
	[Display(Name = "Litologia")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La litología solo puede contener letras y espacios.")]
	public string Litologia { get; set; } = "Desconocida";
}