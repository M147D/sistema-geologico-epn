using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [StringLength(254, ErrorMessage = "El email no puede exceder 254 caracteres")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [RegularExpression(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$", ErrorMessage = "El formato del email no es válido")]
    [NoInjection]
    public string Email { get; set; } = "Sin definir";

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
    [DataType(DataType.Password)]
    [NoInjection]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
    [StringLength(100, ErrorMessage = "La confirmación no puede exceder 100 caracteres")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    [NoInjection]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "El nombre completo no puede exceder 200 caracteres")]
    [NoInjection]
    public string? NombreCompleto { get; set; }

    public RolUsuario Rol { get; set; } = RolUsuario.Free;
}
