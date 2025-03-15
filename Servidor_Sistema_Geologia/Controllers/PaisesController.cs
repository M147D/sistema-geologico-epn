using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servidor_Sistema_Geologia.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaisesController : ControllerBase
	{
		private readonly GestorSistemaGeologia _db;

		public PaisesController(GestorSistemaGeologia db)
		{
			_db = db;
		}

		// GET: api/Paises
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IEnumerable<PaisDto>>> GetPaises()
		{
			try
			{
				var paises = await _db.Paises
					.OrderBy(p => p.NombrePais)
					.ToListAsync();

				// Mapear a DTO para evitar referencias circulares
				var paisesDto = paises.Select(p => new PaisDto
				{
					Id = p.Id,
					NombrePais = p.NombrePais
				}).ToList();

				return Ok(paisesDto);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener países: {ex.Message}");
			}
		}

		// GET: api/Paises/5
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<PaisDto>> GetPais(int id)
		{
			try
			{
				var pais = await _db.Paises.FindAsync(id);

				if (pais == null)
				{
					return NotFound("País no encontrado");
				}

				// Mapear a DTO
				var paisDto = new PaisDto
				{
					Id = pais.Id,
					NombrePais = pais.NombrePais
				};

				return Ok(paisDto);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener el país: {ex.Message}");
			}
		}
	}
}