using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

/// <summary>
/// DTO para la información de ubicación con país y provincia.
/// La provincia es opcional en caso de que solo se conozca el país.
/// </summary>
public class UbicacionCompletoDto
{
    [Required(ErrorMessage = "El nombre del país es obligatorio")]
    [StringLength(100, MinimumLength = 1)]
    [NoInjection]
    public string NombrePais { get; set; } = string.Empty;

    // Provincia es opcional - el servicio usará "Desconocida" o valor por defecto si no se proporciona
    [StringLength(100)]
    [NoInjection]
    public string? NombreProvincia { get; set; }

    [Required(ErrorMessage = "La localidad es obligatoria")]
    [StringLength(500, MinimumLength = 1)]
    [NoInjection]
    public string Localidad { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    [NoInjection]
    public string Latitud { get; set; } = "0.0";

    [Required]
    [StringLength(60)]
    [NoInjection]
    public string Longitud { get; set; } = "0.0";

    [StringLength(60)]
    [NoInjection]
    public string Leyenda { get; set; } = "Sin leyenda";
}
