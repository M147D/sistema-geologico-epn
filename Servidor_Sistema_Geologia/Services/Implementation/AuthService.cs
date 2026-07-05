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

            // Crear nuevo usuario — Rol siempre Free, ignorar lo que venga del cliente
            var usuario = new Servidor_Sistema_Geologia.Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                NombreCompleto = model.NombreCompleto,
                Rol = RolUsuario.Free,
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

            // Verificar bloqueo por intentos fallidos ANTES de comprobar contraseña
            if (await _authRepository.IsLockedOutAsync(usuario))
            {
                _logger.LogWarning("Intento de login con cuenta bloqueada: {Email}", model.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Cuenta bloqueada por demasiados intentos fallidos. Intente de nuevo en 5 minutos."
                };
            }

            var passwordValid = await _authRepository.CheckPasswordAsync(usuario, model.Password);

            if (passwordValid)
            {
                // Resetear contador de fallos al ingresar correctamente
                await _authRepository.ResetAccessFailedCountAsync(usuario);
                _logger.LogInformation("Usuario autenticado exitosamente: {Email}", model.Email);

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
                    TokenExpiration = DateTime.UtcNow.AddMinutes(720)
                };
            }

            // Registrar intento fallido — Identity activa lockout automáticamente al llegar a 3
            await _authRepository.AccessFailedAsync(usuario);
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

}