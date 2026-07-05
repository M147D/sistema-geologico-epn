using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.DTO.Users;

public class CreateUserDto
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [MinLength(3, ErrorMessage = "El nombre de usuario debe tener al menos 3 caracteres")]
    [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "El nombre completo no puede exceder 200 caracteres")]
    public string? NombreCompleto { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio")]
    public RolUsuario Rol { get; set; } = RolUsuario.Free;

    public bool EstadoActivo { get; set; } = true;
}
