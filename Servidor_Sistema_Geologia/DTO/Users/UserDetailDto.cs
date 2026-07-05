namespace Servidor_Sistema_Geologia.DTO.Users;

public class UserDetailDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? NombreCompleto { get; set; }
    public string? PhoneNumber { get; set; }
    public RolUsuario Rol { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoActivo { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
}
