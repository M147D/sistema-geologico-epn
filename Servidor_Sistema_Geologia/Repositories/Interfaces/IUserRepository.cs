using Microsoft.AspNetCore.Identity;
using Servidor_Sistema_Geologia.DTO.Users;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IUserRepository
{
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByUserNameAsync(string userName);
    Task<PaginatedUsersDto> GetAllAsync(UserFilterDto filter);
    Task<IdentityResult> CreateAsync(Usuario user, string password);
    Task<IdentityResult> UpdateAsync(Usuario user);
    Task<IdentityResult> DeleteAsync(Usuario user);
    Task<IdentityResult> ChangePasswordAsync(Usuario user, string newPassword);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUserNameAsync(string userName);
    Task<int> GetTotalCountAsync();
    Task<int> GetActiveCountAsync();
    Task<Dictionary<RolUsuario, int>> GetUserCountByRoleAsync();
}
