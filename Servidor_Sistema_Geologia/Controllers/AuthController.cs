using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.DTO.Auth;
using Servidor_Sistema_Geologia.Services.Interfaces;
using System.Security.Claims;

namespace Servidor_Sistema_Geologia.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto model)
    {
        _logger.LogInformation("Solicitud de registro para email: {Email}", model.Email);

        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Datos de registro inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var resultado = await _authService.RegisterAsync(model);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Autentica un usuario en el sistema
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto model)
    {
        _logger.LogInformation("Solicitud de login para email: {Email}", model.Email);

        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Datos de login inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var resultado = await _authService.LoginAsync(model);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return Unauthorized(resultado);
    }

    /// <summary>
    /// Cierra la sesión del usuario actual
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<AuthResponseDto>> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Solicitud de logout para usuario: {UserId}", userId);

        var resultado = await _authService.LogoutAsync();

        return Ok(resultado);
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado actual
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<AuthResponseDto>> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new AuthResponseDto
            {
                Success = false,
                Message = "Usuario no autenticado"
            });
        }

        var resultado = await _authService.GetCurrentUserAsync(userId);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return Unauthorized(resultado);
    }

    /// <summary>
    /// Cambia la contraseña del usuario autenticado
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<AuthResponseDto>> ChangePassword(ChangePasswordDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new AuthResponseDto
            {
                Success = false,
                Message = "Usuario no autenticado"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Datos inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        _logger.LogInformation("Solicitud de cambio de contraseña para usuario: {UserId}", userId);

        var resultado = await _authService.ChangePasswordAsync(userId, model);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Confirma el email del usuario
    /// </summary>
    [HttpPost("confirm-email")]
    public async Task<ActionResult<AuthResponseDto>> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "UserId y token son requeridos"
            });
        }

        _logger.LogInformation("Solicitud de confirmación de email para usuario: {UserId}", userId);

        var resultado = await _authService.ConfirmEmailAsync(userId, token);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Solicita un enlace de restablecimiento de contraseña
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<AuthResponseDto>> ForgotPassword([FromBody] string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Email es requerido"
            });
        }

        _logger.LogInformation("Solicitud de forgot password para email: {Email}", email);

        var resultado = await _authService.ForgotPasswordAsync(email);

        return Ok(resultado);
    }

    /// <summary>
    /// Restablece la contraseña usando un token
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<AuthResponseDto>> ResetPassword([FromQuery] string email, [FromQuery] string token, [FromBody] string newPassword)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Email, token y nueva contraseña son requeridos"
            });
        }

        _logger.LogInformation("Solicitud de reset password para email: {Email}", email);

        var resultado = await _authService.ResetPasswordAsync(email, token, newPassword);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Valida si el usuario actual está autenticado y activo
    /// </summary>
    [HttpGet("validate")]
    [Authorize]
    public async Task<ActionResult<bool>> ValidateUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(false);
        }

        var isValid = await _authService.ValidateUserAsync(userId);
        return Ok(isValid);
    }



    /// <summary>
    /// Obtiene información del token JWT actual
    /// </summary>
    [HttpGet("token-info")]
    [Authorize]
    public ActionResult GetTokenInfo()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        
        return Ok(new
        {
            Success = true,
            Message = "Información del token obtenida",
            Claims = claims,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            UserName = User.FindFirstValue(ClaimTypes.Name),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}