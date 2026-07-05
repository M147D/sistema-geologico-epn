using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GeologiaGISController : ControllerBase
    {
        private readonly IGeologiaGISService _service;
        private readonly ILogger<GeologiaGISController> _logger;

        public GeologiaGISController(
            IGeologiaGISService service,
            ILogger<GeologiaGISController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("provincias")]
        public async Task<IActionResult> GetProvincias()
        {
            try
            {
                var geoJson = await _service.GetProvinciasGeoJsonAsync();
                return Content(geoJson, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener provincias");
                return StatusCode(500, new { success = false, message = "Error al obtener provincias", error = ex.Message });
            }
        }

        [HttpGet("ecuador")]
        public async Task<IActionResult> GetEcuador()
        {
            try
            {
                var geoJson = await _service.GetEcuadorGeoJsonAsync();
                return Content(geoJson, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contorno de Ecuador");
                return StatusCode(500, new { success = false, message = "Error al obtener contorno de Ecuador", error = ex.Message });
            }
        }

        [HttpGet("geologia/simplified")]
        public async Task<IActionResult> GetSimplifiedGeologia([FromQuery] double tolerance = 0.01)
        {
            try
            {
                var geoJson = await _service.GetGeologiaSimplifiedGeoJsonAsync(tolerance);
                return Content(geoJson, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al simplificar geometrías");
                return StatusCode(500, new { success = false, message = "Error al simplificar geometrías", error = ex.Message });
            }
        }
    }
}
