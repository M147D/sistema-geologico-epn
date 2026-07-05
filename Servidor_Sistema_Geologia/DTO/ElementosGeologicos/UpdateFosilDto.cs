using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class UpdateFosilDto : UpdateElementoGeologicoBaseDto
{
    [Required(ErrorMessage = "El tipo de fósil es obligatorio")]
    [EnumDataType(typeof(SubtipoFosil), ErrorMessage = "El tipo de fósil no es válido")]
    public SubtipoFosil TipoFosil { get; set; } = SubtipoFosil.Desconocido;

    [StringLength(200, ErrorMessage = "La especie no puede exceder los 200 caracteres")]
    [NoInjection]
    public string Especie { get; set; } = "Por determinar";

    [StringLength(200, ErrorMessage = "El período no puede exceder los 200 caracteres")]
    [NoInjection]
    public string Periodo { get; set; } = "Por determinar";
}
