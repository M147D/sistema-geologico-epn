using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class UpdateRocaDto : UpdateElementoGeologicoBaseDto
{
    [Required(ErrorMessage = "El tipo de roca es obligatorio")]
    [EnumDataType(typeof(SubtipoRoca), ErrorMessage = "El tipo de roca no es válido")]
    public SubtipoRoca TipoRoca { get; set; } = SubtipoRoca.Desconocido;

    [StringLength(500, ErrorMessage = "La litologia no puede exceder los 500 caracteres")]
    [NoInjection]
    public string Litologia { get; set; } = "Desconocida";
}
