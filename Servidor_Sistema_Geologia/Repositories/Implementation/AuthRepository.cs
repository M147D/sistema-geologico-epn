using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;

    public AuthRepository(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Usuario?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<Usuario?> FindByUserNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<Usuario?> FindByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IdentityResult> CreateUserAsync(Usuario user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<SignInResult> PasswordSignInAsync(Usuario user, string password, bool rememberMe, bool lockoutOnFailure)
    {
        return await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure);
    }

    public async Task<IdentityResult> ChangePasswordAsync(Usuario user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> CheckPasswordAsync(Usuario user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IdentityResult> UpdateUserAsync(Usuario user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(Usuario user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(Usuario user, string token)
    {
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(Usuario user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string newPassword)
    {
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }
}