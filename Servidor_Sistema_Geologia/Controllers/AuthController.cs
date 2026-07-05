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

}