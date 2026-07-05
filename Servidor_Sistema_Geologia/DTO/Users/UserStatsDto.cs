namespace Servidor_Sistema_Geologia.DTO.Users;

public class UserStatsDto
{
    public int TotalUsuarios { get; set; }
    public int TotalActivos { get; set; }
    public int TotalInactivos { get; set; }
    public Dictionary<string, int> PorRol { get; set; } = new();
}

public class UserStatsResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserStatsDto? Data { get; set; }
}
