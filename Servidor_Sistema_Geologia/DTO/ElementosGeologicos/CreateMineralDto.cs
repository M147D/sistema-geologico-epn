using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class CreateMineralDto : CreateElementoGeologicoBaseDto
{
    [Required(ErrorMessage = "El tipo de mineral es obligatorio")]
    [EnumDataType(typeof(SubtipoMineral), ErrorMessage = "El tipo de mineral no es válido")]
    public SubtipoMineral TipoMineral { get; set; } = SubtipoMineral.Desconocido;

    [StringLength(500, ErrorMessage = "La litologia no puede exceder los 500 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$", ErrorMessage = "La litología solo puede contener letras y espacios")]
    [NoInjection]
    public string Litologia { get; set; } = "Desconocida";
}
