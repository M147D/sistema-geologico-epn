using Servidor_Sistema_Geologia.DTO.Users;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserResponseDto> GetByIdAsync(int id)
    {
        try
        {
            var usuario = await _userRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            return new UserResponseDto
            {
                Success = true,
                Message = "Usuario obtenido exitosamente",
                User = MapToUserDetailDto(usuario)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario por ID: {Id}", id);
            return new UserResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al obtener usuario"
            };
        }
    }

    public async Task<UsersListResponseDto> GetAllAsync(UserFilterDto filter)
    {
        try
        {
            var result = await _userRepository.GetAllAsync(filter);

            return new UsersListResponseDto
            {
                Success = true,
                Message = "Usuarios obtenidos exitosamente",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de usuarios");
            return new UsersListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al obtener usuarios"
            };
        }
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto model)
    {
        try
        {
            // Verificar si el email ya existe
            if (await _userRepository.ExistsByEmailAsync(model.Email))
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Ya existe un usuario con este email",
                    Errors = new List<string> { "Email ya registrado" }
                };
            }

            // Verificar si el username ya existe
            if (await _userRepository.ExistsByUserNameAsync(model.UserName))
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Ya existe un usuario con este nombre de usuario",
                    Errors = new List<string> { "Nombre de usuario ya registrado" }
                };
            }

            var usuario = new Usuario
            {
                UserName = model.UserName,
                Email = model.Email,
                NombreCompleto = model.NombreCompleto,
                Rol = model.Rol,
                FechaCreacion = DateTime.Now,
                EstadoActivo = model.EstadoActivo
            };

            var resultado = await _userRepository.CreateAsync(usuario, model.Password);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("Usuario creado exitosamente: {Email}", model.Email);

                return new UserResponseDto
                {
                    Success = true,
                    Message = "Usuario creado exitosamente",
                    User = MapToUserDetailDto(usuario)
                };
            }

            return new UserResponseDto
            {
                Success = false,
                Message = "Error al crear el usuario",
                Errors = resultado.Errors.Select(e => e.Description).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario: {Email}", model.Email);
            return new UserResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al crear usuario"
            };
        }
    }

    public async Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto model)
    {
        try
        {
            var usuario = await _userRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Verificar si el email ya existe en otro usuario
            if (await EmailExistsAsync(model.Email, id))
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Ya existe otro usuario con este email",
                    Errors = new List<string> { "Email ya registrado por otro usuario" }
                };
            }

            // Verificar si el username ya existe en otro usuario
            if (await UserNameExistsAsync(model.UserName, id))
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Ya existe otro usuario con este nombre de usuario",
                    Errors = new List<string> { "Nombre de usuario ya registrado por otro usuario" }
                };
            }

            // Actualizar propiedades
            usuario.Email = model.Email;
            usuario.UserName = model.UserName;
            usuario.NombreCompleto = model.NombreCompleto;
            usuario.Rol = model.Rol;
            usuario.EstadoActivo = model.EstadoActivo;
            usuario.EmailConfirmed = model.EmailConfirmed;

            var resultado = await _userRepository.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("Usuario actualizado exitosamente: {Id}", id);

                return new UserResponseDto
                {
                    Success = true,
                    Message = "Usuario actualizado exitosamente",
                    User = MapToUserDetailDto(usuario)
                };
            }

            return new UserResponseDto
            {
                Success = false,
                Message = "Error al actualizar el usuario",
                Errors = resultado.Errors.Select(e => e.Description).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario: {Id}", id);
            return new UserResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al actualizar usuario"
            };
        }
    }

    public async Task<UserResponseDto> DeleteAsync(int id)
    {
        try
        {
            var usuario = await _userRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // Verificar si ya está eliminado (inactivo)
            if (!usuario.EstadoActivo)
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "El usuario ya ha sido eliminado previamente"
                };
            }

            var resultado = await _userRepository.DeleteAsync(usuario);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("🔥 Usuario eliminado lógicamente (desactivado): {Id} - {Email}", id, usuario.Email);

                return new UserResponseDto
                {
                    Success = true,
                    Message = "Usuario eliminado exitosamente (mantiene registros para auditoría)"
                };
            }

            return new UserResponseDto
            {
                Success = false,
                Message = "Error al eliminar el usuario",
                Errors = resultado.Errors.Select(e => e.Description).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario: {Id}", id);
            return new UserResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al eliminar usuario"
            };
        }
    }

    public async Task<UserResponseDto> ChangePasswordAsync(int id, ChangeUserPasswordDto model)
    {
        try
        {
            var usuario = await _userRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            var resultado = await _userRepository.ChangePasswordAsync(usuario, model.NewPassword);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("Contraseña cambiada exitosamente para usuario: {Id}", id);

                return new UserResponseDto
                {
                    Success = true,
                    Message = "Contraseña cambiada exitosamente"
                };
            }

            return new UserResponseDto
            {
                Success = false,
                Message = "Error al cambiar la contraseña",
                Errors = resultado.Errors.Select(e => e.Description).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña para usuario: {Id}", id);
            return new UserResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al cambiar contraseña"
            };
        }
    }

    public async Task<UserResponseDto> ToggleActiveStatusAsync(int id)
    {
        try
        {
            var usuario = await _userRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            usuario.EstadoActivo = !usuario.EstadoActivo;
            var resultado = await _userRepository.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                var status = usuario.EstadoActivo ? "activado" : "desactivado";
                _logger.LogInformation("Usuario {Status} exitosamente: {Id}", status, id);

                return new UserResponseDto
                {
                    Success = true,
                    Message = $"Usuario {status} exitosamente",
                    User = MapToUserDetailDto(usuario)
                };
            }

            return new UserResponseDto
            {
                Success = false,
                Message = "Error al cambiar estado del usuario",
                Errors = resultado.Errors.Select(e => e.Description).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del usuario: {Id}", id);
            return new UserResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al cambiar estado"
            };
        }
    }

    public async Task<UserStatsResponseDto> GetUserStatsAsync()
    {
        try
        {
            var totalCount = await _userRepository.GetTotalCountAsync();
            var activeCount = await _userRepository.GetActiveCountAsync();
            var countByRole = await _userRepository.GetUserCountByRoleAsync();

            var porRol = countByRole.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value
            );

            var stats = new UserStatsDto
            {
                TotalUsuarios = totalCount,
                TotalActivos = activeCount,
                TotalInactivos = totalCount - activeCount,
                PorRol = porRol
            };

            return new UserStatsResponseDto
            {
                Success = true,
                Message = "Estadísticas obtenidas exitosamente",
                Data = stats
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de usuarios");
            return new UserStatsResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al obtener estadísticas"
            };
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _userRepository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de usuario: {Id}", id);
            return false;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        try
        {
            if (excludeUserId.HasValue)
            {
                var existingUser = await _userRepository.GetByEmailAsync(email);
                return existingUser != null && existingUser.Id != excludeUserId.Value;
            }

            return await _userRepository.ExistsByEmailAsync(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> UserNameExistsAsync(string userName, int? excludeUserId = null)
    {
        try
        {
            if (excludeUserId.HasValue)
            {
                var existingUser = await _userRepository.GetByUserNameAsync(userName);
                return existingUser != null && existingUser.Id != excludeUserId.Value;
            }

            return await _userRepository.ExistsByUserNameAsync(userName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de username: {UserName}", userName);
            return false;
        }
    }

    private static UserDetailDto MapToUserDetailDto(Usuario usuario)
    {
        return new UserDetailDto
        {
            Id = usuario.Id,
            Email = usuario.Email!,
            UserName = usuario.UserName,
            NombreCompleto = usuario.NombreCompleto,
            PhoneNumber = usuario.PhoneNumber,
            Rol = usuario.Rol,
            FechaCreacion = usuario.FechaCreacion,
            EstadoActivo = usuario.EstadoActivo,
            EmailConfirmed = usuario.EmailConfirmed,
            PhoneNumberConfirmed = usuario.PhoneNumberConfirmed,
            TwoFactorEnabled = usuario.TwoFactorEnabled,
            LockoutEnd = usuario.LockoutEnd.HasValue ? usuario.LockoutEnd.Value.UtcDateTime : (DateTime?)null,
            LockoutEnabled = usuario.LockoutEnabled,
            AccessFailedCount = usuario.AccessFailedCount
        };
    }
}