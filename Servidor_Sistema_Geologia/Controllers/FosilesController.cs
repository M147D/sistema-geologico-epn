using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Application;

namespace Servidor_Sistema_Geologia.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FosilesController : ControllerBase
	{
        private readonly IElementoService<Fosil, FosilDto, FosilDto> _fosilService;

		// Constructor que recibe el servicio en lugar del contexto
        public FosilesController(IElementoService<Fosil, FosilDto, FosilDto> fosilService)
		{
			_fosilService = fosilService ?? throw new ArgumentNullException(nameof(fosilService));
		}

		// GET: api/Fosiles
		[HttpGet]
		[Authorize(Roles = "Free")]
		public async Task<ActionResult<IEnumerable<FosilDto>>> GetFosiles()
		{
			// Usamos el servicio para obtener la lista de fósiles
			var fosiles = await _fosilService.GetAllAsync();
			return Ok(fosiles);
		}

		// GET: api/Fosiles/5
		[HttpGet("{id}")]
		[Authorize(Roles = "Free")]
		public async Task<ActionResult<FosilDto>> GetFosil(int id)
		{
			var fosil = await _fosilService.GetByIdAsync(id);

			if (fosil == null)
			{
				return NotFound();
			}

			return Ok(fosil);
		}

		// PUT: api/Fosiles/5
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutFosil(int id, FosilDto fosilDto)
		{
			if (id != fosilDto.Id)
			{
				return BadRequest();
			}

			var actualizado = await _fosilService.UpdateAsync(id, fosilDto);

			return NotFound();
		}

		// POST: api/Fosiles
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<FosilDto>> PostFosil(FosilDto fosilDto)
		{
			var fosilCreado = await _fosilService.CreateElementoConAccesoAsync(fosilDto, fosilDto.UsuarioId);

			return CreatedAtAction("GetFosil", new { id = fosilCreado.Id }, fosilCreado);
		}

		// DELETE: api/Fosiles/5
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteFosil(int id)
		{
			await _fosilService.DeleteAsync(id);

			return Ok($"Fósil con ID {id} eliminado.");
		}
	}
}