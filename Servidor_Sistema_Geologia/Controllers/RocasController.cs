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
		private readonly IElementoService<Roca, RocaDto, CreateRocaDto> _rocaService;
		private readonly ILogger<RocasController> _logger;

		public RocasController(
			IElementoService<Roca, RocaDto, CreateRocaDto> rocaService,
			ILogger<RocasController> logger)
		{
			_rocaService = rocaService;
			_logger = logger;
		}

		// GET: api/Rocas
		[HttpGet]
		public async Task<ActionResult<IEnumerable<RocaDto>>> GetRocas()
		{
			if (!TryGetUsuarioId(out int usuarioId))
			{
				usuarioId = 1; // Valor por defecto para pruebas
				_logger.LogWarning("No se encontró un ID de usuario en la cookie. Usando valor por defecto: {UsuarioId}", usuarioId);
			}

			var rocas = await _rocaService.GetAllAsync(usuarioId);
			return Ok(rocas);
		}

		// GET: api/Rocas/5
		[HttpGet("{id}")]
		public async Task<ActionResult<RocaDto>> GetRoca(int id)
		{
			if (!TryGetUsuarioId(out int usuarioId))
			{
				usuarioId = 1; // Valor por defecto para pruebas
				_logger.LogWarning("No se encontró un ID de usuario en la cookie. Usando valor por defecto: {UsuarioId}", usuarioId);
			}

			var roca = await _rocaService.GetByIdAsync(id, usuarioId);

			if (roca == null)
			{
				return NotFound();
			}

			return roca;
		}

		// POST: api/Rocas
		[HttpPost]
		public async Task<ActionResult<RocaDto>> PostRoca(CreateRocaDto createRocaDto)
		{
			try
			{
				// Log para depuración
				_logger.LogInformation("Recibiendo datos para crear roca: {@RocaDto}", createRocaDto);

				// Validar datos obligatorios
				if (string.IsNullOrEmpty(createRocaDto.TipoRoca))
				{
					return BadRequest("El tipo de roca es obligatorio");
				}

				if (string.IsNullOrEmpty(createRocaDto.Nombre))
				{
					createRocaDto.Nombre = $"Muestra de {createRocaDto.TipoRoca}";
					_logger.LogInformation("Nombre generado automáticamente: {Nombre}", createRocaDto.Nombre);
				}

				// Asegurarse de que tengamos un usuario
				if (createRocaDto.UsuarioId <= 0)
				{
					if (!TryGetUsuarioId(out int usuarioId))
					{
						usuarioId = 1; // Valor por defecto para pruebas
						_logger.LogWarning("No se encontró un ID de usuario en la cookie. Usando valor por defecto: {UsuarioId}", usuarioId);
					}
					createRocaDto.UsuarioId = usuarioId;
				}

				// Establecer estado por defecto si no se proporciona
				if (string.IsNullOrEmpty(createRocaDto.DescripcionEstado))
				{
					createRocaDto.DescripcionEstado = "Creado";
					_logger.LogInformation("Estado establecido por defecto: {Estado}", createRocaDto.DescripcionEstado);
				}

				// Crear la roca
				_logger.LogInformation("Creando roca con datos: {@RocaDto}", createRocaDto);
				var roca = await _rocaService.CreateElementoConAccesoAsync(createRocaDto, createRocaDto.UsuarioId);

				// Obtener la roca creada
				var rocaDto = await _rocaService.GetByIdAsync(roca.Id, createRocaDto.UsuarioId);

				_logger.LogInformation("Roca creada exitosamente con ID: {Id}", roca.Id);
				return CreatedAtAction(nameof(GetRoca), new { id = roca.Id }, rocaDto);
			}
			catch (System.Exception ex)
			{
				_logger.LogError(ex, "Error al crear la roca");
				return BadRequest($"Error al crear la roca: {ex.Message}");
			}
		}

		// PUT: api/Rocas/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutRoca(int id, CreateRocaDto updateRocaDto)
		{
			if (!TryGetUsuarioId(out int usuarioId))
			{
				usuarioId = 1; // Valor por defecto para pruebas
				_logger.LogWarning("No se encontró un ID de usuario en la cookie. Usando valor por defecto: {UsuarioId}", usuarioId);
			}

			updateRocaDto.UsuarioId = usuarioId;

			try
			{
				_logger.LogInformation("Actualizando roca con ID {Id}: {@RocaDto}", id, updateRocaDto);
				var roca = await _rocaService.UpdateAsync(id, updateRocaDto, usuarioId);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (System.Exception ex)
			{
				_logger.LogError(ex, "Error al actualizar la roca con ID {Id}", id);
				return BadRequest($"Error al actualizar la roca: {ex.Message}");
			}
		}

		// DELETE: api/Rocas/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteRoca(int id)
		{
			if (!TryGetUsuarioId(out int usuarioId))
			{
				usuarioId = 1; // Valor por defecto para pruebas
				_logger.LogWarning("No se encontró un ID de usuario en la cookie. Usando valor por defecto: {UsuarioId}", usuarioId);
			}

			try
			{
				_logger.LogInformation("Eliminando roca con ID {Id}", id);
				await _rocaService.DeleteAsync(id, usuarioId);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (System.Exception ex)
			{
				_logger.LogError(ex, "Error al eliminar la roca con ID {Id}", id);
				return BadRequest($"Error al eliminar la roca: {ex.Message}");
			}
		}

		// Método auxiliar para obtener el ID de usuario de la cookie
		private bool TryGetUsuarioId(out int usuarioId)
		{
			usuarioId = 0;

			// Obtener la cookie de user_id
			if (Request.Cookies.TryGetValue("user_id", out string userIdStr) &&
				int.TryParse(userIdStr, out usuarioId) &&
				usuarioId > 0)
			{
				return true;
			}

			return false;
		}
	}
}