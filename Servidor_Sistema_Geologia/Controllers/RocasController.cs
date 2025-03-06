using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.Application;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servidor_Sistema_Geologia.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RocasController : ControllerBase
	{
		private readonly IElementoService<Roca, RocaDto, CreateRocaDto> _rocaService;

		public RocasController(IElementoService<Roca, RocaDto, CreateRocaDto> rocaService)
		{
			_rocaService = rocaService;
		}

		// GET: api/Rocas
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IEnumerable<RocaDto>>> GetRocas()
		{
			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				var rocas = await _rocaService.GetAllAsync(usuarioId);
				return Ok(rocas);
			}
			catch (KeyNotFoundException)
			{
				return NotFound("No se encontraron rocas");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener las rocas: {ex.Message}");
			}
		}

		// GET: api/Rocas/5
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<RocaDto>> GetRoca(int id)
		{
			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				var roca = await _rocaService.GetByIdAsync(id, usuarioId);

				if (roca == null)
				{
					return NotFound("Roca no encontrada");
				}

				return roca;
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener la roca: {ex.Message}");
			}
		}

		// POST: api/Rocas
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<RocaDto>> PostRoca([FromBody] CreateRocaDto createRocaDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				// Asegurarse de que el UsuarioId esté establecido, o tomarlo de la cookie
				if (createRocaDto.UsuarioId <= 0)
				{
					if (!TryGetUsuarioId(out int usuarioId))
					{
						return Unauthorized("Usuario no autenticado");
					}
					createRocaDto.UsuarioId = usuarioId;
				}

				var roca = await _rocaService.CreateElementoConAccesoAsync(createRocaDto, createRocaDto.UsuarioId);
				var rocaDto = await _rocaService.GetByIdAsync(roca.Id, createRocaDto.UsuarioId);

				return CreatedAtAction(nameof(GetRoca), new { id = roca.Id }, rocaDto);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al crear la roca: {ex.Message}");
			}
		}

		// PUT: api/Rocas/5
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutRoca(int id, [FromBody] CreateRocaDto updateRocaDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				updateRocaDto.UsuarioId = usuarioId;

				var roca = await _rocaService.UpdateAsync(id, updateRocaDto, usuarioId);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound("Roca no encontrada");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al actualizar la roca: {ex.Message}");
			}
		}

		// DELETE: api/Rocas/5
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteRoca(int id)
		{
			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				await _rocaService.DeleteAsync(id, usuarioId);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound("Roca no encontrada");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al eliminar la roca: {ex.Message}");
			}
		}

		// Método auxiliar para obtener el ID de usuario de la cookie
		private bool TryGetUsuarioId(out int usuarioId)
		{
			usuarioId = 0;

			// Obtener la cookie de user_id
			if (Request.Cookies.TryGetValue("user_id", out string? userIdStr) &&
				!string.IsNullOrEmpty(userIdStr) &&
				int.TryParse(userIdStr, out usuarioId) &&
				usuarioId > 0)
			{
				return true;
			}

			return false;
		}
	}
}