using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.DTO.Users;

public class ChangeUserPasswordDto
{
    [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
    [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
