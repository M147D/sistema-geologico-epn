using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class Fosil : ElementoGeologico
{
	[EnumDataType(typeof(SubtipoFosil), ErrorMessage = "El tipo de fósil no es válido.")]
	[Display(Name = "Tipo de Fósil")]
	public SubtipoFosil TipoFosil { get; set; } = SubtipoFosil.Desconocido;
	[MaxLength(200)]
	[Display(Name = "Especie")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La especie solo puede contener letras y espacios.")]
	public string Especie { get; set; } = "Por determinar";

	[MaxLength(200)]
	[Display(Name = "Periodo")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El periodo solo puede contener letras y espacios.")]
	public string Periodo { get; set; } = "Por determinar";
}