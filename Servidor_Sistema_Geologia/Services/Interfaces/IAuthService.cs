using Servidor_Sistema_Geologia.DTO.Auth;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto model);
    Task<AuthResponseDto> LoginAsync(LoginDto model);
    Task<AuthResponseDto> LogoutAsync();
    Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto model);
    Task<AuthResponseDto> GetCurrentUserAsync(string userId);
    Task<AuthResponseDto> ConfirmEmailAsync(string userId, string token);
    Task<AuthResponseDto> ForgotPasswordAsync(string email);
    Task<AuthResponseDto> ResetPasswordAsync(string email, string token, string newPassword);
    Task<bool> ValidateUserAsync(string userId);
}