using Microsoft.AspNetCore.Identity;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<Usuario?> FindByEmailAsync(string email);
    Task<Usuario?> FindByUserNameAsync(string userName);
    Task<Usuario?> FindByIdAsync(string userId);
    Task<IdentityResult> CreateUserAsync(Usuario user, string password);
    Task<SignInResult> PasswordSignInAsync(Usuario user, string password, bool rememberMe, bool lockoutOnFailure);
    Task<IdentityResult> ChangePasswordAsync(Usuario user, string currentPassword, string newPassword);
    Task SignOutAsync();
    Task<bool> CheckPasswordAsync(Usuario user, string password);
    Task<IdentityResult> UpdateUserAsync(Usuario user);
    Task<string> GenerateEmailConfirmationTokenAsync(Usuario user);
    Task<IdentityResult> ConfirmEmailAsync(Usuario user, string token);
    Task<string> GeneratePasswordResetTokenAsync(Usuario user);
    Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string newPassword);
}