using Microsoft.AspNetCore.Identity;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<Servidor_Sistema_Geologia.Usuario?> FindByEmailAsync(string email);
    Task<Servidor_Sistema_Geologia.Usuario?> FindByIdAsync(string userId);
    Task<IdentityResult> CreateUserAsync(Servidor_Sistema_Geologia.Usuario user, string password);
    Task<bool> CheckPasswordAsync(Servidor_Sistema_Geologia.Usuario user, string password);
    Task<bool> IsLockedOutAsync(Servidor_Sistema_Geologia.Usuario user);
    Task<IdentityResult> AccessFailedAsync(Servidor_Sistema_Geologia.Usuario user);
    Task<IdentityResult> ResetAccessFailedCountAsync(Servidor_Sistema_Geologia.Usuario user);
}
