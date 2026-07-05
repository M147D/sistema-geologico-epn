using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

/// <summary>
/// DTO para la información de una foto.
/// </summary>
public class FotoCompletoDto
{
    [Required(ErrorMessage = "La imagen es obligatoria")]
    public byte[] Imagen { get; set; } = Array.Empty<byte>();

    [Required(ErrorMessage = "El tipo de foto es obligatorio")]
    [NoInjection]
    public string TipoFoto { get; set; } = "Desconocido";

    [StringLength(500)]
    [NoInjection]
    public string DescripcionEspecifica { get; set; } = "Vacío";
}
