using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Services.Interfaces;
using System.Security.Claims;

namespace Servidor_Sistema_Geologia.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere autenticación para todas las acciones
public class ElementosGeologicosController : ControllerBase
{
    private readonly IElementoGeologicoService _elementoService;
    private readonly ILogger<ElementosGeologicosController> _logger;

    public ElementosGeologicosController(
        IElementoGeologicoService elementoService,
        ILogger<ElementosGeologicosController> logger)
    {
        _elementoService = elementoService;
        _logger = logger;
    }

    // 🔍 CONSULTAS GENERALES

    /// <summary>
    /// Obtiene todos los elementos geológicos con filtros y paginación
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ElementoGeologicoFilterDto filter)
    {
        try
        {
            var result = await _elementoService.GetAllAsync(filter);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAll elementos geológicos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene todos los elementos geológicos activos (sin paginación)
    /// </summary>
    [HttpGet("activos")]
    public async Task<IActionResult> GetAllActive()
    {
        try
        {
            var result = await _elementoService.GetAllActiveAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAllActive elementos geológicos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un elemento geológico por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var result = await _elementoService.GetByIdAsync(id, usuarioId);
            
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetById elemento geológico ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un elemento geológico con detalles completos (ubicación, galería, fotos)
    /// </summary>
    [HttpGet("{id:int}/detalles")]
    public async Task<IActionResult> GetByIdWithDetails(int id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var result = await _elementoService.GetByIdWithDetailsAsync(id, usuarioId);
            
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetByIdWithDetails elemento geológico ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un elemento geológico por código
    /// </summary>
    [HttpGet("codigo/{codigo}")]
    public async Task<IActionResult> GetByCodigo(string codigo)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var result = await _elementoService.GetByCodigoAsync(codigo, usuarioId);
            
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetByCodigo elemento geológico código: {Codigo}", codigo);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // 🔍 CONSULTAS ESPECÍFICAS POR TIPO

    /// <summary>
    /// Obtiene todos los fósiles con filtros
    /// </summary>
    [HttpGet("fosiles")]
    public async Task<IActionResult> GetFosiles([FromQuery] ElementoGeologicoFilterDto filter)
    {
        try
        {
            var result = await _elementoService.GetFosilesAsync(filter);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetFosiles");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene todos los minerales con filtros
    /// </summary>
    [HttpGet("minerales")]
    public async Task<IActionResult> GetMinerales([FromQuery] ElementoGeologicoFilterDto filter)
    {
        try
        {
            var result = await _elementoService.GetMineralesAsync(filter);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetMinerales");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene todas las rocas con filtros
    /// </summary>
    [HttpGet("rocas")]
    public async Task<IActionResult> GetRocas([FromQuery] ElementoGeologicoFilterDto filter)
    {
        try
        {
            var result = await _elementoService.GetRocasAsync(filter);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetRocas");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // 🔍 CONSULTAS POR UBICACIÓN

    /// <summary>
    /// Obtiene elementos geológicos por ubicación
    /// </summary>
    [HttpGet("ubicacion/{ubicacionId:int}")]
    public async Task<IActionResult> GetByUbicacion(int ubicacionId)
    {
        try
        {
            var result = await _elementoService.GetByUbicacionAsync(ubicacionId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetByUbicacion ID: {UbicacionId}", ubicacionId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene elementos geológicos por país
    /// </summary>
    [HttpGet("pais/{paisId:int}")]
    public async Task<IActionResult> GetByPais(int paisId)
    {
        try
        {
            var result = await _elementoService.GetByPaisAsync(paisId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetByPais ID: {PaisId}", paisId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene elementos geológicos por provincia
    /// </summary>
    [HttpGet("provincia/{provinciaId:int}")]
    public async Task<IActionResult> GetByProvincia(int provinciaId)
    {
        try
        {
            var result = await _elementoService.GetByProvinciaAsync(provinciaId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetByProvincia ID: {ProvinciaId}", provinciaId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ✏️ OPERACIONES CRUD - FÓSILES

    /// <summary>
    /// Crea un nuevo fósil
    /// </summary>
    [HttpPost("fosiles")]
    [Authorize(Roles = "Administrador,Geologo")] // Solo admin y geólogos pueden crear
    public async Task<IActionResult> CreateFosil([FromBody] CreateFosilDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            createDto.UsuarioId = GetCurrentUserId();
            var result = await _elementoService.CreateFosilAsync(createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateFosil: {Nombre}", createDto.Nombre);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un fósil existente
    /// </summary>
    [HttpPut("fosiles/{id:int}")]
    [Authorize(Roles = "Administrador,Geologo")]
    public async Task<IActionResult> UpdateFosil(int id, [FromBody] UpdateFosilDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            updateDto.UsuarioId = GetCurrentUserId();
            var result = await _elementoService.UpdateFosilAsync(id, updateDto);
            
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en UpdateFosil ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ✏️ OPERACIONES CRUD - MINERALES

    /// <summary>
    /// Crea un nuevo mineral
    /// </summary>
    [HttpPost("minerales")]
    [Authorize(Roles = "Administrador,Geologo")]
    public async Task<IActionResult> CreateMineral([FromBody] CreateMineralDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            createDto.UsuarioId = GetCurrentUserId();
            var result = await _elementoService.CreateMineralAsync(createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateMineral: {Nombre}", createDto.Nombre);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un mineral existente
    /// </summary>
    [HttpPut("minerales/{id:int}")]
    [Authorize(Roles = "Administrador,Geologo")]
    public async Task<IActionResult> UpdateMineral(int id, [FromBody] UpdateMineralDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            updateDto.UsuarioId = GetCurrentUserId();
            var result = await _elementoService.UpdateMineralAsync(id, updateDto);
            
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en UpdateMineral ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ✏️ OPERACIONES CRUD - ROCAS

    /// <summary>
    /// Crea una nueva roca
    /// </summary>
    [HttpPost("rocas")]
    [Authorize(Roles = "Administrador,Geologo")]
    public async Task<IActionResult> CreateRoca([FromBody] CreateRocaDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            createDto.UsuarioId = GetCurrentUserId();
            var result = await _elementoService.CreateRocaAsync(createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateRoca: {Nombre}", createDto.Nombre);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza una roca existente
    /// </summary>
    [HttpPut("rocas/{id:int}")]
    [Authorize(Roles = "Administrador,Geologo")]
    public async Task<IActionResult> UpdateRoca(int id, [FromBody] UpdateRocaDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            updateDto.UsuarioId = GetCurrentUserId();
            var result = await _elementoService.UpdateRocaAsync(id, updateDto);
            
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en UpdateRoca ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ✏️ OPERACIONES COMUNES

    /// <summary>
    /// Elimina lógicamente un elemento geológico (soft delete)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")] // Solo administradores pueden eliminar
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var result = await _elementoService.DeleteAsync(id, usuarioId);
            
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Delete elemento geológico ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Restaura un elemento geológico eliminado
    /// </summary>
    [HttpPost("{id:int}/restaurar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Restore(int id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var result = await _elementoService.RestoreAsync(id, usuarioId);
            
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Restore elemento geológico ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ✅ VALIDACIONES

    /// <summary>
    /// Verifica si existe un elemento geológico por ID
    /// </summary>
    [HttpGet("{id:int}/existe")]
    public async Task<IActionResult> Exists(int id)
    {
        try
        {
            var exists = await _elementoService.ExistsAsync(id);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Exists elemento geológico ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Verifica si existe un elemento geológico por código
    /// </summary>
    [HttpGet("codigo/{codigo}/existe")]
    public async Task<IActionResult> ExistsByCodigo(string codigo, [FromQuery] int? excludeId = null)
    {
        try
        {
            var exists = await _elementoService.ExistsByCodigoAsync(codigo, excludeId);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ExistsByCodigo código: {Codigo}", codigo);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Verifica si un elemento puede ser eliminado
    /// </summary>
    [HttpGet("{id:int}/puede-eliminar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CanDelete(int id)
    {
        try
        {
            var canDelete = await _elementoService.CanDeleteAsync(id);
            return Ok(new { canDelete });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CanDelete elemento geológico ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // 📊 ESTADÍSTICAS Y REPORTES

    /// <summary>
    /// Obtiene estadísticas generales del sistema
    /// </summary>
    [HttpGet("estadisticas")]
    [Authorize(Roles = "Administrador,Geologo")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var result = await _elementoService.GetStatsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetStats");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene elementos geológicos recientes
    /// </summary>
    [HttpGet("recientes")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 10)
    {
        try
        {
            var result = await _elementoService.GetRecentAsync(count);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetRecent");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene elementos geológicos eliminados
    /// </summary>
    [HttpGet("eliminados")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetInactive()
    {
        try
        {
            var result = await _elementoService.GetInactiveAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetInactive");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene estadísticas para el dashboard
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "Administrador,Geologo")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var result = await _elementoService.GetDashboardStatsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetDashboardStats");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // 🔍 BÚSQUEDAS AVANZADAS

    /// <summary>
    /// Búsqueda avanzada de elementos geológicos
    /// </summary>
    [HttpGet("buscar")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] ElementoGeologicoFilterDto? filter = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "El término de búsqueda es requerido" });
            }

            var result = await _elementoService.SearchAsync(searchTerm, filter);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Search término: {SearchTerm}", searchTerm);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene elementos geológicos por rango de fechas
    /// </summary>
    [HttpGet("rango-fechas")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
    {
        try
        {
            if (desde > hasta)
            {
                return BadRequest(new { message = "La fecha desde no puede ser mayor que la fecha hasta" });
            }

            var result = await _elementoService.GetByDateRangeAsync(desde, hasta);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetByDateRange desde: {Desde} hasta: {Hasta}", desde, hasta);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene elementos geológicos por donante
    /// </summary>
    [HttpGet("donante")]
    public async Task<IActionResult> GetByDonante([FromQuery] string donante)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(donante))
            {
                return BadRequest(new { message = "El nombre del donante es requerido" });
            }

            var result = await _elementoService.GetByDonanteAsync(donante);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetByDonante: {Donante}", donante);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // 🔄 HISTORIAL

    /// <summary>
    /// Obtiene el historial de acceso de un elemento geológico
    /// </summary>
    [HttpGet("{id:int}/historial")]
    [Authorize(Roles = "Administrador,Geologo")]
    public async Task<IActionResult> GetHistorial(int id)
    {
        try
        {
            var historial = await _elementoService.GetHistorialAsync(id);
            return Ok(new { historial });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetHistorial elemento ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Registra manualmente un acceso en el historial
    /// </summary>
    [HttpPost("{id:int}/registrar-acceso")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegisterAccess(int id, [FromBody] AccionesUsuario accion)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            await _elementoService.RegisterAccessAsync(id, usuarioId, accion);
            return Ok(new { message = "Acceso registrado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en RegisterAccess elemento ID: {Id}, acción: {Accion}", id, accion);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // 🛠️ MÉTODOS AUXILIARES

    /// <summary>
    /// Obtiene el ID del usuario actual desde el token JWT
    /// </summary>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        
        // Si no se puede obtener el ID del usuario, lanzar excepción
        throw new UnauthorizedAccessException("No se pudo obtener el ID del usuario actual");
    }

    /// <summary>
    /// Obtiene el rol del usuario actual
    /// </summary>
    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "Usuario";
    }

    /// <summary>
    /// Verifica si el usuario actual es administrador
    /// </summary>
    private bool IsCurrentUserAdmin()
    {
        return User.IsInRole("Administrador");
    }

    /// <summary>
    /// Verifica si el usuario actual puede editar elementos
    /// </summary>
    private bool CanCurrentUserEdit()
    {
        return User.IsInRole("Administrador") || User.IsInRole("Geologo");
    }
}
