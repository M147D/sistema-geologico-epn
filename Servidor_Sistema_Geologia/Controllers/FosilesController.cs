using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Application; // Asegúrate de tener la referencia a tu servicio

namespace Servidor_Sistema_Geologia.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FosilsController : ControllerBase
	{
        private readonly IElementoService<Fosil, FosilReadDto, FosilCreateDto> _fosilService;

		// Constructor que recibe el servicio en lugar del contexto
        public FosilsController(IElementoService<Fosil, FosilReadDto, FosilCreateDto> fosilService)
		{
			_fosilService = fosilService ?? throw new ArgumentNullException(nameof(fosilService));
		}

		// GET: api/Fosiles
		[HttpGet]
		public async Task<ActionResult<IEnumerable<FosilCreateDto>>> GetFosiles()
		{
			// Usamos el servicio para obtener la lista de fósiles
			var fosiles = await _fosilService.GetAllAsync();
			return Ok(fosiles);
		}

		// GET: api/Fosiles/5
		[HttpGet("{id}")]
		public async Task<ActionResult<FosilCreateDto>> GetFosil(int id)
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
		public async Task<IActionResult> PutFosil(int id, FosilCreateDto fosilDto)
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
		public async Task<ActionResult<FosilCreateDto>> PostFosil(FosilCreateDto fosilDto)
		{
			var fosilCreado = await _fosilService.CreateElementoConAccesoAsync(fosilDto, fosilDto.UsuarioId);

			return CreatedAtAction("GetFosil", new { id = fosilCreado.Id }, fosilCreado);
		}

		// DELETE: api/Fosiles/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFosil(int id)
		{
			await _fosilService.DeleteAsync(id);

			return Ok($"Fósil con ID {id} eliminado.");
		}
	}
}