namespace Servidor_Sistema_Geologia.DTO.Users;

public class UserResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDetailDto? User { get; set; }
    public List<string> Errors { get; set; } = new();
}
