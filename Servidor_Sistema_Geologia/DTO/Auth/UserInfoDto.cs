using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.Auth;

public class UserInfoDto
{
    public int Id { get; set; }

    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [NoInjection]
    public string Email { get; set; } = string.Empty;

    [NoInjection]
    public string? UserName { get; set; }

    [NoInjection]
    public string? NombreCompleto { get; set; }

    public RolUsuario Rol { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoActivo { get; set; }
    public bool EmailConfirmed { get; set; }
}
