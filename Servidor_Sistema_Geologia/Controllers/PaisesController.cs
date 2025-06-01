using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.DTO.Ubicaciones;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaisesController : ControllerBase
{
    private readonly IPaisService _paisService;
    private readonly ILogger<PaisesController> _logger;

    public PaisesController(IPaisService paisService, ILogger<PaisesController> logger)
    {
        _paisService = paisService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene una lista paginada de países con filtros opcionales
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaisesListResponseDto>> GetPaises([FromQuery] PaisFilterDto filter)
    {
        _logger.LogInformation("🌎 Solicitud de lista de países con filtros");

        var resultado = await _paisService.GetAllAsync(filter);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene un país específico por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PaisResponseDto>> GetPais(int id)
    {
        _logger.LogInformation("🌎 Solicitud de país por ID: {Id}", id);

        var resultado = await _paisService.GetByIdAsync(id);

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
    /// Obtiene un país con sus provincias activas
    /// </summary>
    [HttpGet("{id}/with-provincias")]
    public async Task<ActionResult<PaisResponseDto>> GetPaisWithProvincias(int id)
    {
        _logger.LogInformation("🌎 Solicitud de país con provincias por ID: {Id}", id);

        var resultado = await _paisService.GetByIdWithProvinciasAsync(id);

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
    /// Crea un nuevo país
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaisResponseDto>> CreatePais(CreatePaisDto model)
    {
        _logger.LogInformation("🌎 Solicitud de creación de país: {NombrePais}", model.NombrePais);

        if (!ModelState.IsValid)
        {
            return BadRequest(new PaisResponseDto
            {
                Success = false,
                Message = "Datos de creación inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var resultado = await _paisService.CreateAsync(model);

        if (resultado.Success)
        {
            return CreatedAtAction(nameof(GetPais), new { id = resultado.Data!.Id }, resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Actualiza un país existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaisResponseDto>> UpdatePais(int id, UpdatePaisDto model)
    {
        _logger.LogInformation("🌎 Solicitud de actualización de país: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(new PaisResponseDto
            {
                Success = false,
                Message = "Datos de actualización inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var resultado = await _paisService.UpdateAsync(id, model);

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
    /// Elimina un país (SOFT DELETE - solo desactiva, mantiene datos para auditoría)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaisResponseDto>> DeletePais(int id)
    {
        _logger.LogInformation("🗑️ Solicitud de eliminación lógica de país: {Id}", id);

        var resultado = await _paisService.DeleteAsync(id);

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
    /// Restaura un país eliminado (REVERT SOFT DELETE)
    /// </summary>
    [HttpPost("{id}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaisResponseDto>> RestorePais(int id)
    {
        _logger.LogInformation("🔄 Solicitud de restauración de país: {Id}", id);

        var resultado = await _paisService.RestoreAsync(id);

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
    /// Obtiene solo países activos
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<PaisesListResponseDto>> GetActivePaises()
    {
        _logger.LogInformation("🌎 Solicitud de países activos");

        var resultado = await _paisService.GetAllActiveAsync();

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene países eliminados (inactivos)
    /// </summary>
    [HttpGet("inactive")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaisesListResponseDto>> GetInactivePaises()
    {
        _logger.LogInformation("🌎 Solicitud de países eliminados");

        var resultado = await _paisService.GetInactiveAsync();

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene países recientes
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<PaisesListResponseDto>> GetRecentPaises([FromQuery] int count = 10)
    {
        _logger.LogInformation("🌎 Solicitud de países recientes: {Count}", count);

        var resultado = await _paisService.GetRecentAsync(count);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene estadísticas de países
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaisResponseDto>> GetPaisStats()
    {
        _logger.LogInformation("🌎 Solicitud de estadísticas de países");

        var resultado = await _paisService.GetStatsAsync();

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Verifica si un nombre de país ya existe
    /// </summary>
    [HttpGet("check-name/{nombrePais}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CheckPaisNameExists(string nombrePais, [FromQuery] int? excludeId = null)
    {
        var exists = await _paisService.ExistsByNameAsync(nombrePais, excludeId);
        return Ok(exists);
    }

    /// <summary>
    /// Verifica si un país se puede eliminar (no tiene provincias activas)
    /// </summary>
    [HttpGet("{id}/can-delete")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CanDeletePais(int id)
    {
        var canDelete = await _paisService.CanDeleteAsync(id);
        return Ok(canDelete);
    }
}