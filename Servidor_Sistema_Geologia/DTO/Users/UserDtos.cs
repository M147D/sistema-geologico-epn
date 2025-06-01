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

public class UpdateUserDto
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [MinLength(3, ErrorMessage = "El nombre de usuario debe tener al menos 3 caracteres")]
    [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "El nombre completo no puede exceder 200 caracteres")]
    public string? NombreCompleto { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio")]
    public RolUsuario Rol { get; set; }

    public bool EstadoActivo { get; set; }
    public bool EmailConfirmed { get; set; }
}

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

public class UserFilterDto
{
    public string? Email { get; set; }
    public string? NombreCompleto { get; set; }
    public RolUsuario? Rol { get; set; }
    public bool? EstadoActivo { get; set; }
    public bool? EmailConfirmed { get; set; }
    public DateTime? FechaCreacionDesde { get; set; }
    public DateTime? FechaCreacionHasta { get; set; }
    
    // 🔥 CONTROL DE SOFT DELETE
    /// <summary>
    /// Si es true, incluye usuarios inactivos (eliminados lógicamente) en los resultados.
    /// Por defecto es false (solo usuarios activos).
    /// </summary>
    public bool IncludeInactive { get; set; } = false;
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "FechaCreacion";
    public bool SortDescending { get; set; } = true;
}

public class PaginatedUsersDto
{
    public List<UserListDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}

public class ChangeUserPasswordDto
{
    [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
    [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UserResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDetailDto? User { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class UsersListResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaginatedUsersDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}