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
	public class ProvinciasController : ControllerBase
	{
		private readonly GestorSistemaGeologia _db;

		public ProvinciasController(GestorSistemaGeologia db)
		{
			_db = db;
		}

		// GET: api/Provincias
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IEnumerable<ProvinciaDto>>> GetProvincias()
		{
			try
			{
				var provincias = await _db.Provincias
					.OrderBy(p => p.NombreProvincia)
					.ToListAsync();

				// Mapear a DTO para evitar referencias circulares
				var provinciasDto = provincias.Select(p => new ProvinciaDto
				{
					Id = p.Id,
					NombreProvincia = p.NombreProvincia,
					PaisId = p.PaisId
				}).ToList();

				return Ok(provinciasDto);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener provincias: {ex.Message}");
			}
		}

		// GET: api/Provincias/5
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<ProvinciaDto>> GetProvincia(int id)
		{
			try
			{
				var provincia = await _db.Provincias
					.FirstOrDefaultAsync(p => p.Id == id);

				if (provincia == null)
				{
					return NotFound("Provincia no encontrada");
				}

				// Mapear a DTO
				var provinciaDto = new ProvinciaDto
				{
					Id = provincia.Id,
					NombreProvincia = provincia.NombreProvincia,
					PaisId = provincia.PaisId
				};

				return Ok(provinciaDto);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener la provincia: {ex.Message}");
			}
		}

		// GET: api/Provincias/porPais/{paisId}
		[HttpGet("porPais/{paisId}")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<ProvinciaDto>>> GetProvinciasPorPais(int paisId)
		{
			try
			{
				var provincias = await _db.Provincias
					.Where(p => p.PaisId == paisId)
					.OrderBy(p => p.NombreProvincia)
					.ToListAsync();

				// Mapear a DTO
				var provinciasDto = provincias.Select(p => new ProvinciaDto
				{
					Id = p.Id,
					NombreProvincia = p.NombreProvincia,
					PaisId = p.PaisId
				}).ToList();

				return Ok(provinciasDto);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener provincias por país: {ex.Message}");
			}
		}
	}
}