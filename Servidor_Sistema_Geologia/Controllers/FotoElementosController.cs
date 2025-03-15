using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.Application;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Infrastructure;
using Servidor_Sistema_Geologia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Servidor_Sistema_Geologia.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FotoElementosController : ControllerBase
	{
		private readonly IFotoService<FotoElemento, FotoElementoDto, CreateFotoElementoDto> _fotoService;

		public FotoElementosController(IFotoService<FotoElemento, FotoElementoDto, CreateFotoElementoDto> fotoService)
		{
			_fotoService = fotoService;
		}

		// GET: api/FotoElementos
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

		// GET api/FotoElementos/5
		/*[HttpGet("{id}")]
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
		}*/

		// GET api/FotoElementos/imagen/5
		[HttpGet("imagen/{id}")]
		public async Task<ActionResult> GetImagen(int id)
		{
			try
			{
				var foto = await _fotoService.GetByIdAsync(id);
				if (foto == null || foto.Imagen == null)
				{
					return NotFound("Imagen no encontrada");
				}

				// Determinar si el usuario es premium o free
				bool isPremium = IsUserPremium();

				byte[] imagenBytes;

				if (!isPremium)
				{
					// Agregar marca de agua para usuarios free
					imagenBytes = ApplyWatermark(foto.Imagen);
				}
				else
				{
					// No aplicar marca de agua para usuarios premium
					imagenBytes = foto.Imagen;
				}

				// Devolver la imagen como archivo
				return File(imagenBytes, "image/jpeg");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener la imagen: {ex.Message}");
			}
		}

		// GET api/FotoElementos/base64/5
		/*[HttpGet("base64/{id}")]
		public async Task<ActionResult<string>> GetImagenBase64(int id)
		{
			try
			{
				var foto = await _fotoService.GetByIdAsync(id);
				if (foto == null || foto.Imagen == null)
				{
					return NotFound("Imagen no encontrada");
				}

				// Determinar si el usuario es premium o free
				bool isPremium = IsUserPremium();

				byte[] imagenBytes;

				if (!isPremium)
				{
					// Agregar marca de agua para usuarios free
					imagenBytes = ApplyWatermark(foto.Imagen);
				}
				else
				{
					// No aplicar marca de agua para usuarios premium
					imagenBytes = foto.Imagen;
				}

				// Convertir a base64 para uso directo en HTML img
				string base64 = Convert.ToBase64String(imagenBytes);
				return Ok(base64);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (System.Exception ex)
			{
				return BadRequest($"Error al obtener la imagen: {ex.Message}");
			}
		}*/

		// POST api/FotoElementos/galeria/{galeriaId}
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

				// Crear un DTO simplificado para la respuesta que no incluya la imagen
				var respuestaFoto = new
				{
					foto.Id,
					foto.GaleriaElementosGeologicoId,
					foto.TipoFoto,
					foto.FechaSubida,
					foto.CreadoPor,
					foto.DescripcionEspecifica,
					foto.Etiquetas,
					ImagenUrl = $"/api/FotoElementos/imagen/{foto.Id}"
				};

				return Ok(respuestaFoto);
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

		// PUT api/FotoElementos/5
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

				// Crear un DTO simplificado para la respuesta que no incluya la imagen
				var respuestaFoto = new
				{
					foto.Id,
					foto.GaleriaElementosGeologicoId,
					foto.TipoFoto,
					foto.FechaSubida,
					foto.CreadoPor,
					foto.DescripcionEspecifica,
					foto.Etiquetas,
					ImagenUrl = $"/api/FotoElementos/imagen/{foto.Id}"
				};

				return Ok(respuestaFoto);
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

		// DELETE api/FotoElementos/5
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

		// Método auxiliar para determinar si el usuario tiene rol premium
		private bool IsUserPremium()
		{
			// Implementar lógica para verificar si el usuario tiene un rol premium
			// Por ejemplo:
			return User.IsInRole("Premium") || User.IsInRole("Admin");
		}

		// Método para aplicar múltiples marcas de agua a una imagen
		private byte[] ApplyWatermark(byte[] imageBytes)
		{
			try
			{
				using (MemoryStream inStream = new MemoryStream(imageBytes))
				using (Image image = Image.FromStream(inStream))
				using (Bitmap bitmap = new Bitmap(image))
				using (Graphics graphics = Graphics.FromImage(bitmap))
				using (MemoryStream outStream = new MemoryStream())
				{
					// Configuraciones para la marca de agua
					string watermarkText = "MuseoGeologico";

					// Aplicar múltiples marcas de agua en diferentes posiciones y ángulos
					ApplyMultipleWatermarks(graphics, bitmap.Width, bitmap.Height, watermarkText);

					// Guardar la imagen con marcas de agua
					bitmap.Save(outStream, ImageFormat.Jpeg);
					return outStream.ToArray();
				}
			}
			catch
			{
				// En caso de error, devolver la imagen original
				return imageBytes;
			}
		}

		// Método auxiliar para aplicar múltiples marcas de agua
		private void ApplyMultipleWatermarks(Graphics graphics, int imageWidth, int imageHeight, string text)
		{
			// Configurar calidad del gráfico
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

			// Configurar fuentes para diferentes tamaños de marca de agua
			using (Font fontLarge = new Font("Arial", 36, FontStyle.Bold))
			using (Font fontMedium = new Font("Arial", 24, FontStyle.Bold))
			using (Font fontSmall = new Font("Arial", 16, FontStyle.Bold))
			{
				// Configurar diferentes pinceles con distintos niveles de transparencia
				using (SolidBrush brushMain = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
				using (SolidBrush brushSecondary = new SolidBrush(Color.FromArgb(70, 200, 200, 200)))
				{
					// 1. Marca de agua grande en el centro
					SizeF textSizeLarge = graphics.MeasureString(text, fontLarge);
					float xCenter = (imageWidth - textSizeLarge.Width) / 2;
					float yCenter = (imageHeight - textSizeLarge.Height) / 2;
					graphics.DrawString(text, fontLarge, brushMain, xCenter, yCenter);

					// 2. Patrón de marca de agua en toda la imagen
					CreateWatermarkPattern(graphics, imageWidth, imageHeight, text, fontSmall, brushSecondary);
				}
			}
		}

		// Método para crear un patrón de marcas de agua por toda la imagen
		private void CreateWatermarkPattern(Graphics graphics, int imageWidth, int imageHeight, string text, Font font, Brush brush)
		{
			SizeF textSize = graphics.MeasureString(text, font);
			int spacingX = (int)textSize.Width + 50;
			int spacingY = (int)textSize.Height + 30;

			// Determinar el número de filas y columnas para el patrón
			int columns = (int)Math.Ceiling((double)imageWidth / spacingX) + 1;
			int rows = (int)Math.Ceiling((double)imageHeight / spacingY) + 1;

			// Crear un desplazamiento para que el patrón no sea perfectamente alineado
			int offsetX = 0;

			// Dibujar el patrón de marca de agua
			for (int row = -1; row < rows; row++)
			{
				// Alternar el desplazamiento en cada fila para crear un patrón escalonado
				offsetX = (row % 2 == 0) ? 0 : spacingX / 2;

				for (int col = -1; col < columns; col++)
				{
					float x = col * spacingX + offsetX;
					float y = row * spacingY;

					graphics.DrawString(text, font, brush, x, y);
				}
			}
		}
	}
}