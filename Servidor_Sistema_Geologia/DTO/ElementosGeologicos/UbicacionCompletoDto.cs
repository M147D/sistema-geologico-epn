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

    // Grados decimales; vacío si se desconoce (REV-01, H-06).
    [StringLength(60)]
    [Coordenada(-90, 90)]
    public string? Latitud { get; set; }

    // Grados decimales; vacío si se desconoce (REV-01, H-06).
    [StringLength(60)]
    [Coordenada(-180, 180)]
    public string? Longitud { get; set; }

    [StringLength(60)]
    [NoInjection]
    public string Leyenda { get; set; } = "Sin leyenda";
}
