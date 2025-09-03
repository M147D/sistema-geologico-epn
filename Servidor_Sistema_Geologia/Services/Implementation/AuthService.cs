using Servidor_Sistema_Geologia.DTO.Auth;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthRepository authRepository, 
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
    {
        try
        {
            // Verificar si el usuario ya existe
            var existingUser = await _authRepository.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Ya existe un usuario con este email",
                    Errors = new List<string> { "Email ya registrado" }
                };
            }

            // Crear nuevo usuario
            var usuario = new Servidor_Sistema_Geologia.Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                NombreCompleto = model.NombreCompleto,
                Rol = model.Rol,
                FechaCreacion = DateTime.Now,
                EstadoActivo = true
            };

            var resultado = await _authRepository.CreateUserAsync(usuario, model.Password);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("Usuario registrado exitosamente: {Email}", model.Email);

                // 🔥 GENERAR JWT TOKEN (12 HORAS)
                var token = _jwtService.GenerateToken(usuario);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Usuario registrado exitosamente",
                    User = new UserInfoDto
                    {
                        Id = usuario.Id,
                        Email = usuario.Email!,
                        UserName = usuario.UserName,
                        NombreCompleto = usuario.NombreCompleto,
                        Rol = usuario.Rol,
                        FechaCreacion = usuario.FechaCreacion,
                        EstadoActivo = usuario.EstadoActivo,
                        EmailConfirmed = usuario.EmailConfirmed
                    },
                    Token = token,
                    TokenExpiration = DateTime.UtcNow.AddMinutes(720) // 12 horas
                };
            }

            return new AuthResponseDto
            {
                Success = false,
                Message = "Error al registrar el usuario",
                Errors = resultado.Errors.Select(e => e.Description).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el registro de usuario para email: {Email}", model.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error interno del servidor durante el registro"
            };
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto model)
    {
        try
        {
            var usuario = await _authRepository.FindByEmailAsync(model.Email);
            if (usuario == null)
            {
                _logger.LogWarning("Intento de login con email no existente: {Email}", model.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                };
            }

            if (!usuario.EstadoActivo)
            {
                _logger.LogWarning("Intento de login con usuario desactivado: {Email}", model.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Usuario desactivado"
                };
            }

            // 🔥 VERIFICAR CONTRASEÑA SIN SIGN IN
            var passwordValid = await _authRepository.CheckPasswordAsync(usuario, model.Password);
            
            if (passwordValid)
            {
                _logger.LogInformation("Usuario autenticado exitosamente: {Email}", model.Email);

                // 🔥 GENERAR JWT TOKEN (12 HORAS)
                var token = _jwtService.GenerateToken(usuario);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login exitoso",
                    User = new UserInfoDto
                    {
                        Id = usuario.Id,
                        Email = usuario.Email!,
                        UserName = usuario.UserName,
                        NombreCompleto = usuario.NombreCompleto,
                        Rol = usuario.Rol,
                        FechaCreacion = usuario.FechaCreacion,
                        EstadoActivo = usuario.EstadoActivo,
                        EmailConfirmed = usuario.EmailConfirmed
                    },
                    Token = token,
                    TokenExpiration = DateTime.UtcNow.AddMinutes(720) // 12 horas
                };
            }

            _logger.LogWarning("Credenciales inválidas para: {Email}", model.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Credenciales inválidas"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login para email: {Email}", model.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error interno del servidor durante el login"
            };
        }
    }

    public Task<AuthResponseDto> LogoutAsync()
    {
        try
        {
            // El token simplemente expira o el cliente lo descarta
            _logger.LogInformation("Usuario cerró sesión exitosamente");

            return Task.FromResult(new AuthResponseDto
            {
                Success = true,
                Message = "Logout exitoso"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el logout");
            return Task.FromResult(new AuthResponseDto
            {
                Success = false,
                Message = "Error interno del servidor durante el logout"
            });
        }
    }

    public async Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto model)
    {
        try
        {
            // 🔥 USAR FindByIdAsync PARA JWT
            var usuario = await _authRepository.FindByIdAsync(userId);
            if (usuario == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            var resultado = await _authRepository.ChangePasswordAsync(usuario, model.CurrentPassword, model.NewPassword);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("Contraseña cambiada exitosamente para usuario: {Email}", usuario.Email);

                // GENERAR NUEVO TOKEN DESPUÉS DEL CAMBIO (12 HORAS)
                var token = _jwtService.GenerateToken(usuario);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Contraseña cambiada exitosamente",
                    Token = token,
                    TokenExpiration = DateTime.UtcNow.AddMinutes(720)
                };
            }

            return new AuthResponseDto
            {
                Success = false,
                Message = "Error al cambiar la contraseña",
                Errors = resultado.Errors.Select(e => e.Description).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña para usuario: {UserId}", userId);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al cambiar contraseña"
            };
        }
    }

    public async Task<AuthResponseDto> GetCurrentUserAsync(string userId)
    {
        try
        {
            // 🔥 USAR FindByIdAsync PARA JWT
            var usuario = await _authRepository.FindByIdAsync(userId);
            if (usuario == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "Usuario obtenido exitosamente",
                User = new UserInfoDto
                {
                    Id = usuario.Id,
                    Email = usuario.Email!,
                    UserName = usuario.UserName,
                    NombreCompleto = usuario.NombreCompleto,
                    Rol = usuario.Rol,
                    FechaCreacion = usuario.FechaCreacion,
                    EstadoActivo = usuario.EstadoActivo,
                    EmailConfirmed = usuario.EmailConfirmed
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario actual: {UserId}", userId);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al obtener usuario"
            };
        }
    }

    public async Task<AuthResponseDto> ConfirmEmailAsync(string userId, string token)
    {
        try
        {
            // 🔥 USAR FindByIdAsync PARA JWT
            var usuario = await _authRepository.FindByIdAsync(userId);
            if (usuario == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            var resultado = await _authRepository.ConfirmEmailAsync(usuario, token);

            if (resultado.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Email confirmado exitosamente"
                };
            }

            return new AuthResponseDto
            {
                Success = false,
                Message = "Error al confirmar email",
                Errors = resultado.Errors.Select(e => e.Description).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al confirmar email para usuario: {UserId}", userId);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al confirmar email"
            };
        }
    }

    public async Task<AuthResponseDto> ForgotPasswordAsync(string email)
    {
        try
        {
            var usuario = await _authRepository.FindByEmailAsync(email);
            if (usuario == null)
            {
                // Por seguridad, no revelar si el email existe o no
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Si el email existe, se enviará un enlace de recuperación"
                };
            }

            var token = await _authRepository.GeneratePasswordResetTokenAsync(usuario);
            
            // Aquí implementarías el envío de email
            // await _emailService.SendPasswordResetEmailAsync(usuario.Email, token);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Se ha enviado un enlace de recuperación al email proporcionado"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en forgot password para email: {Email}", email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error interno del servidor"
            };
        }
    }

    public async Task<AuthResponseDto> ResetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            var usuario = await _authRepository.FindByEmailAsync(email);
            if (usuario == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            var resultado = await _authRepository.ResetPasswordAsync(usuario, token, newPassword);

            if (resultado.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Contraseña restablecida exitosamente"
                };
            }

            return new AuthResponseDto
            {
                Success = false,
                Message = "Error al restablecer contraseña",
                Errors = resultado.Errors.Select(e => e.Description).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restablecer contraseña para email: {Email}", email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al restablecer contraseña"
            };
        }
    }



    public async Task<bool> ValidateUserAsync(string userId)
    {
        try
        {
            // 🔥 USAR FindByIdAsync PARA JWT
            var usuario = await _authRepository.FindByIdAsync(userId);
            return usuario != null && usuario.EstadoActivo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar usuario: {UserId}", userId);
            return false;
        }
    }
}