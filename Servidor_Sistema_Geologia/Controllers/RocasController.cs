using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Application;

namespace Servidor_Sistema_Geologia.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RocasController : ControllerBase
	{
		private readonly IElementoService<Roca, RocaDto, RocaDto> _rocaService;
		private readonly ILogger<RocasController> _logger;

		public RocasController(IElementoService<Roca, RocaDto, RocaDto> rocaService, ILogger<RocasController> logger)
		{
			_rocaService = rocaService ?? throw new ArgumentNullException(nameof(rocaService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		// Método privado para obtener el ID del usuario actual
		private int GetCurrentUserId()
		{
			if (!Request.Cookies.TryGetValue("user_id", out var userIdStr) ||
				!int.TryParse(userIdStr, out var userId))
			{
				throw new UnauthorizedAccessException("Usuario no autenticado o ID no válido");
			}
			return userId;
		}

		// GET: api/Rocas
		[HttpGet]
		[Authorize(Roles = "Free")]
		public async Task<ActionResult<IEnumerable<RocaDto>>> GetRocas()
		{
			try
			{
				int usuarioId = GetCurrentUserId();
				var rocas = await _rocaService.GetAllAsync(usuarioId);
				return Ok(rocas);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error al obtener rocas");
				return StatusCode(500, new { message = "Error al obtener rocas" });
			}
		}

		// GET: api/Rocas/5
		[HttpGet("{id}")]
		[Authorize(Roles = "Free")]
		public async Task<ActionResult<RocaDto>> GetRoca(int id)
		{
			try
			{
				int usuarioId = GetCurrentUserId();
				var roca = await _rocaService.GetByIdAsync(id, usuarioId);

				if (roca == null)
				{
					return NotFound();
				}

				return Ok(roca);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error al obtener roca {Id}", id);
				return StatusCode(500, new { message = "Error al obtener roca" });
			}
		}

		// PUT: api/Rocas/5
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutRoca(int id, RocaDto rocaDto)
		{
			if (id != rocaDto.Id)
			{
				return BadRequest();
			}

			try
			{
				int usuarioId = GetCurrentUserId();
				var actualizado = await _rocaService.UpdateAsync(id, rocaDto, usuarioId);
				return Ok(actualizado);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error al actualizar roca {Id}", id);
				return StatusCode(500, new { message = "Error al actualizar roca" });
			}
		}

		// POST: api/Rocas
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<RocaDto>> PostRoca(RocaDto rocaDto)
		{
			try
			{
				int usuarioId = GetCurrentUserId();
				var rocaCreada = await _rocaService.CreateElementoConAccesoAsync(rocaDto, usuarioId);
				return CreatedAtAction("GetRoca", new { id = rocaCreada.Id }, rocaCreada);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error al crear roca");
				return StatusCode(500, new { message = "Error al crear roca" });
			}
		}

		// DELETE: api/Rocas/5
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteRoca(int id)
		{
			try
			{
				int usuarioId = GetCurrentUserId();
				await _rocaService.DeleteAsync(id, usuarioId);
				return Ok($"Roca con ID {id} eliminada.");
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error al eliminar roca {Id}", id);
				return StatusCode(500, new { message = "Error al eliminar roca" });
			}
		}
	}
}