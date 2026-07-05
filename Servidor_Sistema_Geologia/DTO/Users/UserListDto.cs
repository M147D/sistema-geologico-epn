namespace Servidor_Sistema_Geologia.DTO.Users;

public class UserListDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? NombreCompleto { get; set; }
    public RolUsuario Rol { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoActivo { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
