using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.Auth;

public class AuthResponseDto
{
    public bool Success { get; set; }

    [NoInjection]
    public string Message { get; set; } = string.Empty;

    public UserInfoDto? User { get; set; }

    [NoInjection]
    public string? Token { get; set; }

    public DateTime? TokenExpiration { get; set; }
    public List<string> Errors { get; set; } = new();
}
