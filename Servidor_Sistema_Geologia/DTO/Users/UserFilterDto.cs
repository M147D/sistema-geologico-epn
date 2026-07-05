namespace Servidor_Sistema_Geologia.DTO.Users;

public class UserFilterDto
{
    public string? Email { get; set; }
    public string? NombreCompleto { get; set; }
    public RolUsuario? Rol { get; set; }
    public bool? EstadoActivo { get; set; }
    public bool? EmailConfirmed { get; set; }
    public DateTime? FechaCreacionDesde { get; set; }
    public DateTime? FechaCreacionHasta { get; set; }

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
