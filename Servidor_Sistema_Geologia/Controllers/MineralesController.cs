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
	public class MineralesController : ControllerBase
	{
		private readonly IElementoService<Mineral, MineralDto, CreateMineralDto> _mineralService;

		public MineralesController(IElementoService<Mineral, MineralDto, CreateMineralDto> mineralService)
		{
			_mineralService = mineralService;
		}

		// GET: api/Minerales
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IEnumerable<MineralDto>>> GetMinerales()
		{
			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				var minerales = await _mineralService.GetAllAsync();
				return Ok(minerales);
			}
			catch (KeyNotFoundException)
			{
				return NotFound("No se encontraron minerales");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener los minerales: {ex.Message}");
			}
		}

		// GET: api/Minerales/5
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<MineralDto>> GetMineral(int id)
		{
			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				var mineral = await _mineralService.GetByIdAsync(id, usuarioId);

				if (mineral == null)
				{
					return NotFound();
				}

				return mineral;
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener el mineral: {ex.Message}");
			}
		}

		// GET: api/Minerales/filtro
		[HttpGet("filtro")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<MineralDto>>> FiltrarMinerales(
			[FromQuery] int? paisId,
			[FromQuery] int? provinciaId,
			[FromQuery] string? localidad,
			[FromQuery] string? nombre,
			[FromQuery] string? tipo)
		{
			try
			{

				var filtro = new FiltroElementoDto
				{
					PaisId = paisId,
					ProvinciaId = provinciaId,
					Localidad = localidad,
					Nombre = nombre,
					Tipo = tipo
				};

				var minerales = await _mineralService.FilterAsync(filtro);
				return Ok(minerales);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al filtrar los minerales: {ex.Message}");
			}
		}

		// También podemos mantener el endpoint POST para filtrar con un cuerpo JSON
		[HttpPost("filtrar")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<MineralDto>>> FiltrarMineralesPost([FromBody] FiltroElementoDto filtro)
		{
			try
			{
				var minerales = await _mineralService.FilterAsync(filtro);
				return Ok(minerales);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al filtrar los minerales: {ex.Message}");
			}
		}

		// POST: api/Minerales
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<MineralDto>> PostMineral([FromBody] CreateMineralDto createMineralDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				// Asegurarse de que el UsuarioId esté establecido, o tomarlo de la cookie
				if (createMineralDto.UsuarioId <= 0)
				{
					if (!TryGetUsuarioId(out int usuarioId))
					{
						return Unauthorized("Usuario no autenticado");
					}
					createMineralDto.UsuarioId = usuarioId;
				}

				var mineral = await _mineralService.CreateElementoConAccesoAsync(createMineralDto, createMineralDto.UsuarioId);
				var mineralDto = await _mineralService.GetByIdAsync(mineral.Id, createMineralDto.UsuarioId);
				//Reemplazar por un OK
				return CreatedAtAction(nameof(GetMineral), new { id = mineral.Id }, mineralDto);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al crear el mineral: {ex.Message}");
			}
		}

		// PUT: api/Minerales/5
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutMineral(int id, [FromBody] CreateMineralDto updateMineralDto)
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

				updateMineralDto.UsuarioId = usuarioId;

				var mineral = await _mineralService .UpdateAsync(id, updateMineralDto, usuarioId);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound("Mineral no encontrado");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al actualizar el mineral: {ex.Message}");
			}
		}

		// DELETE: api/Minerales/5
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteMineral(int id)
		{
			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				await _mineralService.DeleteAsync(id, usuarioId);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound("Mineral no encontrado");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al eliminar el mineral: {ex.Message}");
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