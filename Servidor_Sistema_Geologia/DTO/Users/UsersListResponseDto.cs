namespace Servidor_Sistema_Geologia.DTO.Users;

public class UsersListResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaginatedUsersDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}
