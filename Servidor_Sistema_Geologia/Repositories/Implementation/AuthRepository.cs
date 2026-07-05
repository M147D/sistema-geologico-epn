using Microsoft.AspNetCore.Identity;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<Servidor_Sistema_Geologia.Usuario> _userManager;

    public AuthRepository(UserManager<Servidor_Sistema_Geologia.Usuario> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Servidor_Sistema_Geologia.Usuario?> FindByEmailAsync(string email)
        => await _userManager.FindByEmailAsync(email);

    public async Task<Servidor_Sistema_Geologia.Usuario?> FindByIdAsync(string userId)
        => await _userManager.FindByIdAsync(userId);

    public async Task<IdentityResult> CreateUserAsync(Servidor_Sistema_Geologia.Usuario user, string password)
        => await _userManager.CreateAsync(user, password);

    public Task<bool> CheckPasswordAsync(Servidor_Sistema_Geologia.Usuario user, string password)
        => _userManager.CheckPasswordAsync(user, password);

    public Task<bool> IsLockedOutAsync(Servidor_Sistema_Geologia.Usuario user)
        => _userManager.IsLockedOutAsync(user);

    public Task<IdentityResult> AccessFailedAsync(Servidor_Sistema_Geologia.Usuario user)
        => _userManager.AccessFailedAsync(user);

    public Task<IdentityResult> ResetAccessFailedCountAsync(Servidor_Sistema_Geologia.Usuario user)
        => _userManager.ResetAccessFailedCountAsync(user);
}
