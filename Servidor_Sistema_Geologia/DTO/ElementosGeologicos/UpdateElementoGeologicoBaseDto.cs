using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public abstract class UpdateElementoGeologicoBaseDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 200 caracteres")]
    [NoInjection]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La edad es obligatoria")]
    [StringLength(50, ErrorMessage = "La edad no puede exceder los 50 caracteres")]
    [NoInjection]
    public string Edad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El donante es obligatorio")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El donante debe tener entre 1 y 200 caracteres")]
    [NoInjection]
    public string Donante { get; set; } = string.Empty;

    [DataType(DataType.Date, ErrorMessage = "La fecha de ingreso debe ser una fecha válida")]
    public DateTime? FechaIngreso { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [RegularExpression(@"^[a-zA-Z0-9.]+$", ErrorMessage = "El código solo puede contener letras, números y puntos")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El código debe tener entre 1 y 100 caracteres")]
    [NoInjection]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de ejemplares es obligatorio")]
    [Range(1, 10000, ErrorMessage = "El número de ejemplares debe estar entre 1 y 10000")]
    public uint Ejemplares { get; set; } = 1;

    [StringLength(5000, ErrorMessage = "Los documentos relacionados no pueden exceder los 5000 caracteres")]
    [NoInjection]
    public string? DocumentosRelacionados { get; set; }

    [Display(Name = "¿Existe lámina?")]
    public bool LaminaExiste { get; set; } = false;

    public int? UbicacionId { get; set; }

    // Campos de ubicación para edición directa (opcional, se usan si UbicacionId es null o para actualizar la ubicación existente)
    [StringLength(100)]
    [NoInjection]
    public string? NombrePais { get; set; }

    [StringLength(100)]
    [NoInjection]
    public string? NombreProvincia { get; set; }

    [StringLength(500)]
    [NoInjection]
    public string? Localidad { get; set; }

    [StringLength(60)]
    [NoInjection]
    public string? Latitud { get; set; }

    [StringLength(60)]
    [NoInjection]
    public string? Longitud { get; set; }

    public int UsuarioId { get; set; } // Para el historial de acceso
}
