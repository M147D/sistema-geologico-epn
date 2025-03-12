using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.Application;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Infrastructure;
using Servidor_Sistema_Geologia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Servidor_Sistema_Geologia.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FotoElementosController : ControllerBase
	{
		public readonly IFotoService<FotoElemento, FotoElementoDto, CreateFotoElementoDto> _fotoService;

		public FotoElementosController(IFotoService<FotoElemento, FotoElementoDto, CreateFotoElementoDto> fotoService)
		{
			_fotoService = fotoService;
		}

		// GET: api/<FotoElementosController>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<FotoElementoDto>>> GetFotos()
		{
			try
			{
				var fotos = await _fotoService.GetAllAsync();
				return Ok(fotos);
			}
			catch (KeyNotFoundException)
			{
				return NotFound("No se encontraron fotos");
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener las fotos: {ex.Message}");
			}
		}

		// GET api/<FotoElementosController>/5
		[HttpGet("{id}")]
		public async Task<ActionResult<FotoElementoDto>> GetFoto(int id)
		{
			try
			{
				var foto = await _fotoService.GetByIdAsync(id);
				return Ok(foto);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener la foto: {ex.Message}");
			}
		}

		// POST api/<FotoElementosController>/galeria/{galeriaId}
		[HttpPost("galeria/{galeriaId}")]
		public async Task<ActionResult<FotoElemento>> CreateFoto(int galeriaId, [FromForm] CreateFotoElementoDto fotoDto)
		{
			try
			{
				string userName = "Sistema";
				if (User.Identity.IsAuthenticated && User.Identity.Name != null)
				{
					userName = User.Identity.Name;
				}

				// Procesar el archivo de imagen y convertirlo a byte[]
				if (fotoDto.ImagenFile != null && fotoDto.ImagenFile.Length > 0)
				{
					using (var memoryStream = new MemoryStream())
					{
						await fotoDto.ImagenFile.CopyToAsync(memoryStream);
						fotoDto.Imagen = memoryStream.ToArray();
					}
				}

				var foto = await _fotoService.CreateAsync(fotoDto, galeriaId, userName);
				/*var fotodto = await _fotoService.GetByIdAsync(foto.Id);

				return CreatedAtAction(nameof(GetFoto), new { id = foto.Id }, fotodto);*/
				return Ok(foto);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al crear la foto: {ex.Message}");
			}
		}

		// PUT api/<FotoElementosController>/5
		[HttpPut("{id}")]
		public async Task<ActionResult<FotoElemento>> UpdateFoto(int id, [FromForm] CreateFotoElementoDto fotoDto)
		{
			try
			{
				int usuarioId = 0;
				// Aquí podrías obtener el ID del usuario autenticado si lo necesitas

				// Procesar el archivo de imagen y convertirlo a byte[] si se proporciona
				if (fotoDto.ImagenFile != null && fotoDto.ImagenFile.Length > 0)
				{
					using (var memoryStream = new MemoryStream())
					{
						await fotoDto.ImagenFile.CopyToAsync(memoryStream);
						fotoDto.Imagen = memoryStream.ToArray();
					}
				}

				var foto = await _fotoService.UpdateAsync(id, fotoDto, usuarioId);
				return Ok(foto);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al actualizar la foto: {ex.Message}");
			}
		}

		// DELETE api/<FotoElementosController>/5
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteFoto(int id)
		{
			try
			{
				int usuarioId = 0;
				// Aquí podrías obtener el ID del usuario autenticado si lo necesitas

				await _fotoService.DeleteAsync(id, usuarioId);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al eliminar la foto: {ex.Message}");
			}
		}
	}
}