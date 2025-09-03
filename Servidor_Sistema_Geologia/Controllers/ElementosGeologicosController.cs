using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Services.Interfaces;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using System.Security.Claims;

namespace Servidor_Sistema_Geologia.Controllers;

[Route("api/elementos-geologicos")]
[ApiController]
public class ElementosGeologicosController : ControllerBase
{
    private readonly IFosilService _fosilService;
    private readonly IMineralService _mineralService;
    private readonly IRocaService _rocaService;
    private readonly IElementoGeologicoRepository _elementoRepository;

    public ElementosGeologicosController(
        IFosilService fosilService, 
        IMineralService mineralService, 
        IRocaService rocaService,
        IElementoGeologicoRepository elementoRepository)
    {
        _fosilService = fosilService;
        _mineralService = mineralService;
        _rocaService = rocaService;
        _elementoRepository = elementoRepository;
    }

    // ===========================================
    // ENDPOINTS GENERALES (TODOS LOS ELEMENTOS)
    // ===========================================

    /// <summary>
    /// Obtiene todos los elementos geológicos, opcionalmente filtrados por tipo
    /// </summary>
    /// <param name="tipo">Filtro opcional por tipo: "Fosil", "Mineral", "Roca"</param>
    [HttpGet]
    [Authorize(Roles = "Admin,Premium")]
    public async Task<ActionResult> GetAll([FromQuery] string? tipo = null)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var filter = new ElementoGeologicoFilterDto();

            var result = tipo?.ToLower() switch
            {
                "fosil" => await _fosilService.GetAllAsync(filter),
                "mineral" => await _mineralService.GetAllAsync(filter),
                "roca" => await _rocaService.GetAllAsync(filter),
                _ => await GetAllElements(filter)
            };

            return Ok(result.ElementosGeologicos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener elementos geológicos: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene un elemento geológico por ID (detecta automáticamente el tipo)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Premium")]
    public async Task<ActionResult> GetById(int id)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            // Intentamos obtener el elemento de cada servicio hasta encontrarlo
            var fosilResult = await _fosilService.GetByIdAsync(id, usuarioId);
            if (fosilResult.Success)
                return Ok(fosilResult.Data);

            var mineralResult = await _mineralService.GetByIdAsync(id, usuarioId);
            if (mineralResult.Success)
                return Ok(mineralResult.Data);

            var rocaResult = await _rocaService.GetByIdAsync(id, usuarioId);
            if (rocaResult.Success)
                return Ok(rocaResult.Data);

            return NotFound("Elemento geológico no encontrado");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener elemento geológico: {ex.Message}");
        }
    }

    // ===========================================
    // ENDPOINTS ESPECÍFICOS POR TIPO
    // ===========================================

    /// <summary>
    /// Obtiene todos los fósiles
    /// </summary>
    [HttpGet("fosiles")]
    [Authorize(Roles = "Admin,Premium")]
    public async Task<ActionResult> GetFosiles()
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _fosilService.GetAllAsync(new ElementoGeologicoFilterDto());
            return Ok(result.ElementosGeologicos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener fósiles: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene todos los minerales
    /// </summary>
    [HttpGet("minerales")]
    [Authorize(Roles = "Admin,Premium")]
    public async Task<ActionResult> GetMinerales()
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _mineralService.GetAllAsync(new ElementoGeologicoFilterDto());
            return Ok(result.ElementosGeologicos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener minerales: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene todas las rocas
    /// </summary>
    [HttpGet("rocas")]
    [Authorize(Roles = "Admin,Premium")]
    public async Task<ActionResult> GetRocas()
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _rocaService.GetAllAsync(new ElementoGeologicoFilterDto());
            return Ok(result.ElementosGeologicos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener rocas: {ex.Message}");
        }
    }

    // ===========================================
    // ENDPOINTS DE CREACIÓN POR TIPO
    // ===========================================

    /// <summary>
    /// Crea un nuevo fósil
    /// </summary>
    [HttpPost("fosiles")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateFosil(CreateFosilDto createDto)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _fosilService.CreateAsync(createDto, usuarioId);
            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear fósil: {ex.Message}");
        }
    }

    /// <summary>
    /// Crea un nuevo mineral
    /// </summary>
    [HttpPost("minerales")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateMineral(CreateMineralDto createDto)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _mineralService.CreateAsync(createDto, usuarioId);
            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear mineral: {ex.Message}");
        }
    }

    /// <summary>
    /// Crea una nueva roca
    /// </summary>
    [HttpPost("rocas")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateRoca(CreateRocaDto createDto)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _rocaService.CreateAsync(createDto, usuarioId);
            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear roca: {ex.Message}");
        }
    }

    // ===========================================
    // ENDPOINTS DE ACTUALIZACIÓN POR TIPO
    // ===========================================

    /// <summary>
    /// Actualiza un fósil específico
    /// </summary>
    [HttpPut("fosiles/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateFosil(int id, UpdateFosilDto updateDto)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _fosilService.UpdateAsync(id, updateDto, usuarioId);
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                    return NotFound(result.Message);
                return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar fósil: {ex.Message}");
        }
    }

    /// <summary>
    /// Actualiza un mineral específico
    /// </summary>
    [HttpPut("minerales/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateMineral(int id, UpdateMineralDto updateDto)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _mineralService.UpdateAsync(id, updateDto, usuarioId);
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                    return NotFound(result.Message);
                return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar mineral: {ex.Message}");
        }
    }

    /// <summary>
    /// Actualiza una roca específica
    /// </summary>
    [HttpPut("rocas/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateRoca(int id, UpdateRocaDto updateDto)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _rocaService.UpdateAsync(id, updateDto, usuarioId);
            if (!result.Success)
            {
                if (result.Message.Contains("no encontrado"))
                    return NotFound(result.Message);
                return BadRequest(new { success = false, message = result.Message, errors = result.Errors });
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar roca: {ex.Message}");
        }
    }

    // ===========================================
    // ENDPOINTS DE ELIMINACIÓN
    // ===========================================

    /// <summary>
    /// Elimina un elemento geológico por ID (detecta automáticamente el tipo)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            if (!TryGetUsuarioId(out int usuarioId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            // Intentamos eliminar del servicio correspondiente
            if (await _fosilService.DeleteAsync(id, usuarioId))
                return Ok(new { success = true, message = "Fósil eliminado exitosamente" });

            if (await _mineralService.DeleteAsync(id, usuarioId))
                return Ok(new { success = true, message = "Mineral eliminado exitosamente" });

            if (await _rocaService.DeleteAsync(id, usuarioId))
                return Ok(new { success = true, message = "Roca eliminada exitosamente" });

            return NotFound("Elemento geológico no encontrado");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar elemento geológico: {ex.Message}");
        }
    }

    // ===========================================
    // MÉTODOS AUXILIARES PRIVADOS
    // ===========================================

    /// <summary>
    /// Obtiene todos los elementos geológicos de todos los tipos combinados
    /// </summary>
    private async Task<PaginatedElementosGeologicosDto> GetAllElements(ElementoGeologicoFilterDto filter)
    {
        // Use the unified repository to get all elements at once
        return await _elementoRepository.GetAllAsync(filter);
    }


    /// <summary>
    /// Obtiene el ID del usuario desde el token JWT
    /// </summary>
    private bool TryGetUsuarioId(out int usuarioId)
    {
        usuarioId = 0;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out usuarioId);
    }
}