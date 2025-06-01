using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.DTO.Users;
using Servidor_Sistema_Geologia.Services.Interfaces;
using System.Security.Claims;

namespace Servidor_Sistema_Geologia.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene una lista paginada de usuarios con filtros opcionales
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsersListResponseDto>> GetUsers([FromQuery] UserFilterDto filter)
    {
        _logger.LogInformation("Solicitud de lista de usuarios con filtros");

        var resultado = await _userService.GetAllAsync(filter);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene un usuario específico por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> GetUser(int id)
    {
        _logger.LogInformation("Solicitud de usuario por ID: {Id}", id);

        var resultado = await _userService.GetByIdAsync(id);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        if (resultado.Message.Contains("no encontrado"))
        {
            return NotFound(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene un usuario por email
    /// </summary>
    [HttpGet("by-email/{email}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> GetUserByEmail(string email)
    {
        _logger.LogInformation("Solicitud de usuario por email: {Email}", email);

        var resultado = await _userService.GetByEmailAsync(email);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        if (resultado.Message.Contains("no encontrado"))
        {
            return NotFound(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto model)
    {
        _logger.LogInformation("Solicitud de creación de usuario: {Email}", model.Email);

        if (!ModelState.IsValid)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Message = "Datos de creación inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var resultado = await _userService.CreateAsync(model);

        if (resultado.Success)
        {
            return CreatedAtAction(nameof(GetUser), new { id = resultado.User!.Id }, resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, UpdateUserDto model)
    {
        _logger.LogInformation("Solicitud de actualización de usuario: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Message = "Datos de actualización inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var resultado = await _userService.UpdateAsync(id, model);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        if (resultado.Message.Contains("no encontrado"))
        {
            return NotFound(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Elimina un usuario (SOFT DELETE - solo desactiva, mantiene datos para auditoría)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> DeleteUser(int id)
    {
        _logger.LogInformation("🔥 Solicitud de eliminación lógica de usuario: {Id}", id);

        // Verificar que no se esté eliminando a sí mismo
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == id.ToString())
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Message = "No puedes eliminar tu propia cuenta"
            });
        }

        var resultado = await _userService.DeleteAsync(id);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        if (resultado.Message.Contains("no encontrado"))
        {
            return NotFound(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Cambia la contraseña de un usuario específico (solo admin)
    /// </summary>
    [HttpPost("{id}/change-password")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> ChangeUserPassword(int id, ChangeUserPasswordDto model)
    {
        _logger.LogInformation("Solicitud de cambio de contraseña para usuario: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Message = "Datos inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var resultado = await _userService.ChangePasswordAsync(id, model);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        if (resultado.Message.Contains("no encontrado"))
        {
            return NotFound(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Activa o desactiva un usuario
    /// </summary>
    [HttpPost("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> ToggleUserStatus(int id)
    {
        _logger.LogInformation("Solicitud de cambio de estado para usuario: {Id}", id);

        // Verificar que no se esté desactivando a sí mismo
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == id.ToString())
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Message = "No puedes cambiar el estado de tu propia cuenta"
            });
        }

        var resultado = await _userService.ToggleActiveStatusAsync(id);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        if (resultado.Message.Contains("no encontrado"))
        {
            return NotFound(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Confirma el email de un usuario
    /// </summary>
    [HttpPost("{id}/confirm-email")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> ConfirmUserEmail(int id)
    {
        _logger.LogInformation("Solicitud de confirmación de email para usuario: {Id}", id);

        var resultado = await _userService.ConfirmEmailAsync(id);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        if (resultado.Message.Contains("no encontrado"))
        {
            return NotFound(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Bloquea o desbloquea un usuario
    /// </summary>
    [HttpPost("{id}/lockout")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> SetUserLockout(int id, [FromBody] DateTimeOffset? lockoutEnd)
    {
        _logger.LogInformation("Solicitud de bloqueo para usuario: {Id}", id);

        // Verificar que no se esté bloqueando a sí mismo
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == id.ToString())
        {
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Message = "No puedes bloquear tu propia cuenta"
            });
        }

        var resultado = await _userService.SetLockoutAsync(id, lockoutEnd);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        if (resultado.Message.Contains("no encontrado"))
        {
            return NotFound(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene usuarios por rol
    /// </summary>
    [HttpGet("by-role/{role}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsersListResponseDto>> GetUsersByRole(RolUsuario role)
    {
        _logger.LogInformation("Solicitud de usuarios por rol: {Role}", role);

        var resultado = await _userService.GetByRoleAsync(role);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene solo usuarios activos
    /// </summary>
    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsersListResponseDto>> GetActiveUsers()
    {
        _logger.LogInformation("Solicitud de usuarios activos");

        var resultado = await _userService.GetActiveUsersAsync();

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene usuarios eliminados (inactivos)
    /// </summary>
    [HttpGet("deleted")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsersListResponseDto>> GetDeletedUsers([FromQuery] int count = 10)
    {
        _logger.LogInformation("Solicitud de usuarios eliminados: {Count}", count);

        // Crear filtro que incluye inactivos y excluye activos
        var filter = new UserFilterDto
        {
            EstadoActivo = false, // Solo usuarios inactivos
            IncludeInactive = true,
            PageSize = count
        };

        var resultado = await _userService.GetAllAsync(filter);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Reactivar un usuario eliminado (REVERT SOFT DELETE)
    /// </summary>
    [HttpPost("{id}/reactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> ReactivateUser(int id)
    {
        _logger.LogInformation("🔄 Solicitud de reactivación de usuario: {Id}", id);

        // Usar ToggleActiveStatus que ya existe para reactivar
        var resultado = await _userService.ToggleActiveStatusAsync(id);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        if (resultado.Message.Contains("no encontrado"))
        {
            return NotFound(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene usuarios recientes (solo activos)
    /// </summary>
    [HttpGet("recent")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsersListResponseDto>> GetRecentUsers([FromQuery] int count = 10)
    {
        _logger.LogInformation("Solicitud de usuarios recientes: {Count}", count);

        var resultado = await _userService.GetRecentUsersAsync(count);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene estadísticas de usuarios
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> GetUserStats()
    {
        _logger.LogInformation("Solicitud de estadísticas de usuarios");

        var resultado = await _userService.GetUserStatsAsync();

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Verifica si un email ya existe
    /// </summary>
    [HttpGet("check-email/{email}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CheckEmailExists(string email, [FromQuery] int? excludeUserId = null)
    {
        var exists = await _userService.EmailExistsAsync(email, excludeUserId);
        return Ok(exists);
    }

    /// <summary>
    /// Verifica si un username ya existe
    /// </summary>
    [HttpGet("check-username/{userName}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CheckUserNameExists(string userName, [FromQuery] int? excludeUserId = null)
    {
        var exists = await _userService.UserNameExistsAsync(userName, excludeUserId);
        return Ok(exists);
    }
}