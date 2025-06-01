using Servidor_Sistema_Geologia.DTO.Users;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> GetByIdAsync(int id);
    Task<UserResponseDto> GetByEmailAsync(string email);
    Task<UsersListResponseDto> GetAllAsync(UserFilterDto filter);
    Task<UserResponseDto> CreateAsync(CreateUserDto model);
    Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto model);
    Task<UserResponseDto> DeleteAsync(int id);
    Task<UserResponseDto> ChangePasswordAsync(int id, ChangeUserPasswordDto model);
    Task<UserResponseDto> ToggleActiveStatusAsync(int id);
    Task<UserResponseDto> ConfirmEmailAsync(int id);
    Task<UserResponseDto> SetLockoutAsync(int id, DateTimeOffset? lockoutEnd);
    Task<UsersListResponseDto> GetByRoleAsync(RolUsuario role);
    Task<UsersListResponseDto> GetActiveUsersAsync();
    Task<UsersListResponseDto> GetRecentUsersAsync(int count = 10);
    Task<UserResponseDto> GetUserStatsAsync();
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    Task<bool> UserNameExistsAsync(string userName, int? excludeUserId = null);
}