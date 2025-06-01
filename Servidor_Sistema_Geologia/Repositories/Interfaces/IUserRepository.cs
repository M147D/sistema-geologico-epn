using Microsoft.AspNetCore.Identity;
using Servidor_Sistema_Geologia.DTO.Users;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IUserRepository
{
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByUserNameAsync(string userName);
    Task<PaginatedUsersDto> GetAllAsync(UserFilterDto filter);
    Task<List<Usuario>> GetAllActiveAsync();
    Task<List<Usuario>> GetByRoleAsync(RolUsuario role); // Solo usuarios activos
    Task<List<Usuario>> GetByRoleAsync(RolUsuario role, bool includeInactive); // Con opción de incluir inactivos
    Task<IdentityResult> CreateAsync(Usuario user, string password);
    Task<IdentityResult> UpdateAsync(Usuario user);
    Task<IdentityResult> DeleteAsync(Usuario user); // Soft delete - cambia EstadoActivo = false
    Task<IdentityResult> ChangePasswordAsync(Usuario user, string newPassword);
    Task<IdentityResult> SetEmailConfirmedAsync(Usuario user, bool confirmed);
    Task<IdentityResult> SetLockoutEnabledAsync(Usuario user, bool enabled);
    Task<IdentityResult> SetLockoutEndDateAsync(Usuario user, DateTimeOffset? lockoutEnd);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUserNameAsync(string userName);
    Task<int> GetTotalCountAsync();
    Task<int> GetActiveCountAsync();
    Task<int> GetDeletedCountAsync(); // Cuenta usuarios eliminados (inactivos)
    Task<List<Usuario>> GetRecentUsersAsync(int count = 10);
    Task<List<Usuario>> GetRecentDeletedUsersAsync(int count = 10); // Usuarios eliminados recientemente
    Task<Dictionary<RolUsuario, int>> GetUserCountByRoleAsync();
}