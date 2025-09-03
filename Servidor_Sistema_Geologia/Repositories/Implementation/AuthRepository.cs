using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<Servidor_Sistema_Geologia.Usuario> _userManager;
    private readonly SignInManager<Servidor_Sistema_Geologia.Usuario> _signInManager;

    public AuthRepository(UserManager<Servidor_Sistema_Geologia.Usuario> userManager, SignInManager<Servidor_Sistema_Geologia.Usuario> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Servidor_Sistema_Geologia.Usuario?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<Servidor_Sistema_Geologia.Usuario?> FindByUserNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<Servidor_Sistema_Geologia.Usuario?> FindByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IdentityResult> CreateUserAsync(Servidor_Sistema_Geologia.Usuario user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<SignInResult> PasswordSignInAsync(Servidor_Sistema_Geologia.Usuario user, string password, bool rememberMe, bool lockoutOnFailure)
    {
        return await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure);
    }

    public async Task<IdentityResult> ChangePasswordAsync(Servidor_Sistema_Geologia.Usuario user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> CheckPasswordAsync(Servidor_Sistema_Geologia.Usuario user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IdentityResult> UpdateUserAsync(Servidor_Sistema_Geologia.Usuario user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(Servidor_Sistema_Geologia.Usuario user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(Servidor_Sistema_Geologia.Usuario user, string token)
    {
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(Servidor_Sistema_Geologia.Usuario user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(Servidor_Sistema_Geologia.Usuario user, string token, string newPassword)
    {
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }
}