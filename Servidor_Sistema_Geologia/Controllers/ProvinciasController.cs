using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.DTO.Ubicaciones;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProvinciasController : ControllerBase
{
    private readonly IProvinciaService _provinciaService;
    private readonly ILogger<ProvinciasController> _logger;

    public ProvinciasController(IProvinciaService provinciaService, ILogger<ProvinciasController> logger)
    {
        _provinciaService = provinciaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene una lista paginada de provincias con filtros opcionales
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ProvinciasListResponseDto>> GetProvincias([FromQuery] ProvinciaFilterDto filter)
    {
        _logger.LogInformation("🏞️ Solicitud de lista de provincias con filtros");

        var resultado = await _provinciaService.GetAllAsync(filter);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene una provincia específica por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProvinciaResponseDto>> GetProvincia(int id)
    {
        _logger.LogInformation("🏞️ Solicitud de provincia por ID: {Id}", id);

        var resultado = await _provinciaService.GetByIdAsync(id);

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
    /// Obtiene una provincia con información del país
    /// </summary>
    [HttpGet("{id}/with-pais")]
    public async Task<ActionResult<ProvinciaResponseDto>> GetProvinciaWithPais(int id)
    {
        _logger.LogInformation("🏞️ Solicitud de provincia con información del país por ID: {Id}", id);

        var resultado = await _provinciaService.GetByIdWithPaisAsync(id);

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
    /// Obtiene provincias por país
    /// </summary>
    [HttpGet("by-pais/{paisId}")]
    public async Task<ActionResult<ProvinciasListResponseDto>> GetProvinciasByPais(int paisId, [FromQuery] bool includeInactive = false)
    {
        _logger.LogInformation("🏞️ Solicitud de provincias por país: {PaisId}", paisId);

        var resultado = await _provinciaService.GetByPaisIdAsync(paisId, includeInactive);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Crea una nueva provincia
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProvinciaResponseDto>> CreateProvincia(CreateProvinciaDto model)
    {
        _logger.LogInformation("🏞️ Solicitud de creación de provincia: {NombreProvincia}", model.NombreProvincia);

        if (!ModelState.IsValid)
        {
            return BadRequest(new ProvinciaResponseDto
            {
                Success = false,
                Message = "Datos de creación inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var resultado = await _provinciaService.CreateAsync(model);

        if (resultado.Success)
        {
            return CreatedAtAction(nameof(GetProvincia), new { id = resultado.Data!.Id }, resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Actualiza una provincia existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProvinciaResponseDto>> UpdateProvincia(int id, UpdateProvinciaDto model)
    {
        _logger.LogInformation("🏞️ Solicitud de actualización de provincia: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(new ProvinciaResponseDto
            {
                Success = false,
                Message = "Datos de actualización inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var resultado = await _provinciaService.UpdateAsync(id, model);

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
    /// Elimina una provincia (SOFT DELETE - solo desactiva, mantiene datos para auditoría)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProvinciaResponseDto>> DeleteProvincia(int id)
    {
        _logger.LogInformation("🗑️ Solicitud de eliminación lógica de provincia: {Id}", id);

        var resultado = await _provinciaService.DeleteAsync(id);

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
    /// Restaura una provincia eliminada (REVERT SOFT DELETE)
    /// </summary>
    [HttpPost("{id}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProvinciaResponseDto>> RestoreProvincia(int id)
    {
        _logger.LogInformation("🔄 Solicitud de restauración de provincia: {Id}", id);

        var resultado = await _provinciaService.RestoreAsync(id);

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
    /// Obtiene solo provincias activas
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<ProvinciasListResponseDto>> GetActiveProvincias()
    {
        _logger.LogInformation("🏞️ Solicitud de provincias activas");

        var resultado = await _provinciaService.GetAllActiveAsync();

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene provincias eliminadas (inactivas)
    /// </summary>
    [HttpGet("inactive")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProvinciasListResponseDto>> GetInactiveProvincias()
    {
        _logger.LogInformation("🏞️ Solicitud de provincias eliminadas");

        var resultado = await _provinciaService.GetInactiveAsync();

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene provincias recientes
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<ProvinciasListResponseDto>> GetRecentProvincias([FromQuery] int count = 10)
    {
        _logger.LogInformation("🏞️ Solicitud de provincias recientes: {Count}", count);

        var resultado = await _provinciaService.GetRecentAsync(count);

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Obtiene estadísticas de provincias
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProvinciaResponseDto>> GetProvinciaStats()
    {
        _logger.LogInformation("🏞️ Solicitud de estadísticas de provincias");

        var resultado = await _provinciaService.GetStatsAsync();

        if (resultado.Success)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    /// <summary>
    /// Verifica si un nombre de provincia ya existe en un país
    /// </summary>
    [HttpGet("check-name/{nombreProvincia}/pais/{paisId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CheckProvinciaNameExists(string nombreProvincia, int paisId, [FromQuery] int? excludeId = null)
    {
        var exists = await _provinciaService.ExistsByNameInPaisAsync(nombreProvincia, paisId, excludeId);
        return Ok(exists);
    }

    /// <summary>
    /// Verifica si una provincia se puede eliminar (no tiene ubicaciones activas)
    /// </summary>
    [HttpGet("{id}/can-delete")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CanDeleteProvincia(int id)
    {
        var canDelete = await _provinciaService.CanDeleteAsync(id);
        return Ok(canDelete);
    }
}