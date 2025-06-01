using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.ElementosGeologicos;

public class Mineral : ElementoGeologico
{
	[EnumDataType(typeof(SubtipoMineral), ErrorMessage = "El tipo de mineral no es válido.")]
	[Display(Name = "Tipo de Mineral")]
	public SubtipoMineral TipoMineral { get; set; } = SubtipoMineral.Desconocido;
	[MaxLength(200)]
	[Display(Name = "Litología")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La litología solo puede contener letras y espacios.")]
	public string Litologia { get; set; } = "Desconocida";
}
