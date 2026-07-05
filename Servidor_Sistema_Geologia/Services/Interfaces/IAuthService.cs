using Servidor_Sistema_Geologia.DTO.Auth;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto model);
    Task<AuthResponseDto> LoginAsync(LoginDto model);
    Task<AuthResponseDto> LogoutAsync();
    Task<AuthResponseDto> GetCurrentUserAsync(string userId);
}
