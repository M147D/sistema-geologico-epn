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
	public class FosilesController : ControllerBase
	{
		private readonly IElementoService<Fosil, FosilDto, CreateFosilDto> _fosilService;

		public FosilesController(IElementoService<Fosil, FosilDto, CreateFosilDto> fosilService)
		{
			_fosilService = fosilService;
		}

		// GET: api/Fosiles
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IEnumerable<FosilDto>>> GetFosiles()
		{
			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				var fosiles = await _fosilService.GetAllAsync(usuarioId);
				return Ok(fosiles);
			}
			catch (KeyNotFoundException)
			{
				return NotFound("No se encontraron fósiles");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener los fósiles: {ex.Message}");
			}
		}

		// GET: api/Fosiles/5
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<FosilDto>> GetFosil(int id)
		{
			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				var fosil = await _fosilService.GetByIdAsync(id, usuarioId);

				if (fosil == null)
				{
					return NotFound();
				}

				return fosil;
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener el fósil: {ex.Message}");
			}
		}

		// POST: api/Fosiles
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<FosilDto>> PostFosil([FromBody] CreateFosilDto createFosilDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				// Asegurarse de que el UsuarioId esté establecido, o tomarlo de la cookie
				if (createFosilDto.UsuarioId <= 0)
				{
					if (!TryGetUsuarioId(out int usuarioId))
					{
						return Unauthorized("Usuario no autenticado");
					}
					createFosilDto.UsuarioId = usuarioId;
				}

				var fosil = await _fosilService.CreateElementoConAccesoAsync(createFosilDto, createFosilDto.UsuarioId);
				//var fosilDto = await _fosilService.GetByIdAsync(fosil.Id, createFosilDto.UsuarioId);

				return CreatedAtAction(nameof(GetFosil), new { id = fosil.Id }/*, fosilDto*/);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al crear el fósil: {ex.Message}");
			}
		}

		// PUT: api/Fosiles/5
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutFosil(int id, [FromBody] CreateFosilDto updateFosilDto)
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

				updateFosilDto.UsuarioId = usuarioId;

				var fosil = await _fosilService.UpdateAsync(id, updateFosilDto, usuarioId);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound("Fósil no encontrado");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al actualizar el fósil: {ex.Message}");
			}
		}

		// DELETE: api/Fosiles/5
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteFosil(int id)
		{
			try
			{
				// Obtener ID de usuario de la cookie
				if (!TryGetUsuarioId(out int usuarioId))
				{
					return Unauthorized("Usuario no autenticado");
				}

				await _fosilService.DeleteAsync(id, usuarioId);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound("Fósil no encontrado");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al eliminar el fósil: {ex.Message}");
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