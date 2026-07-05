using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.Gallery;

public class UpdateFotoElementoDto
{
    public byte[]? Imagen { get; set; }

    [Required(ErrorMessage = "El tipo de foto es obligatorio")]
    public TipoFoto TipoFoto { get; set; } = TipoFoto.Desconocido;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s,.-]*$", ErrorMessage = "La descripción solo puede contener letras, números, espacios y algunos caracteres especiales")]
    [NoInjection]
    public string DescripcionEspecifica { get; set; } = "Vacío";
}
