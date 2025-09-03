using Microsoft.AspNetCore.Identity;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<Servidor_Sistema_Geologia.Usuario?> FindByEmailAsync(string email);
    Task<Servidor_Sistema_Geologia.Usuario?> FindByUserNameAsync(string userName);
    Task<Servidor_Sistema_Geologia.Usuario?> FindByIdAsync(string userId);
    Task<IdentityResult> CreateUserAsync(Servidor_Sistema_Geologia.Usuario user, string password);
    Task<SignInResult> PasswordSignInAsync(Servidor_Sistema_Geologia.Usuario user, string password, bool rememberMe, bool lockoutOnFailure);
    Task<IdentityResult> ChangePasswordAsync(Servidor_Sistema_Geologia.Usuario user, string currentPassword, string newPassword);
    Task SignOutAsync();
    Task<bool> CheckPasswordAsync(Servidor_Sistema_Geologia.Usuario user, string password);
    Task<IdentityResult> UpdateUserAsync(Servidor_Sistema_Geologia.Usuario user);
    Task<string> GenerateEmailConfirmationTokenAsync(Servidor_Sistema_Geologia.Usuario user);
    Task<IdentityResult> ConfirmEmailAsync(Servidor_Sistema_Geologia.Usuario user, string token);
    Task<string> GeneratePasswordResetTokenAsync(Servidor_Sistema_Geologia.Usuario user);
    Task<IdentityResult> ResetPasswordAsync(Servidor_Sistema_Geologia.Usuario user, string token, string newPassword);
}