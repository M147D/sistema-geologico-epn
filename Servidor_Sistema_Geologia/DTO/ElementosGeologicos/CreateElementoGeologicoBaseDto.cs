using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public abstract class CreateElementoGeologicoBaseDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
    [NoInjection]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La edad es obligatoria")]
    [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s.]+$", ErrorMessage = "La edad solo puede contener letras, numeros y espacios")]
    [StringLength(50, ErrorMessage = "La edad no puede exceder los 50 caracteres")]
    [NoInjection]
    public string Edad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El donante es obligatorio")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El donante debe tener entre 1 y 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El donante solo puede contener letras y espacios")]
    [NoInjection]
    public string Donante { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
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
    [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s,;.-]*$", ErrorMessage = "Los documentos relacionados solo pueden contener letras, números, espacios y algunos caracteres especiales")]
    [NoInjection]
    public string? DocumentosRelacionados { get; set; }

    [Display(Name = "¿Existe lámina?")]
    public bool LaminaExiste { get; set; } = false;

    public int? UbicacionId { get; set; }

    public int UsuarioId { get; set; } // Para el historial de acceso

    // INFORMACIÓN DE UBICACIÓN
    [Required(ErrorMessage = "La información de ubicación es obligatoria")]
    public UbicacionCompletoDto? Ubicacion { get; set; }

    // Location properties for convenience (filled from Ubicacion)
    public string? Latitud => Ubicacion?.Latitud;
    public string? Longitud => Ubicacion?.Longitud;
    public string? Localidad => Ubicacion?.Localidad;
    public string? NombrePais => Ubicacion?.NombrePais;
    public string? NombreProvincia => Ubicacion?.NombreProvincia;
    public string? Leyenda => Ubicacion?.Leyenda;
}
