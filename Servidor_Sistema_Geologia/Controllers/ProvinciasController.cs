using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.DTO.Locations;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProvinciasController : ControllerBase
{
    private readonly IProvinciaService _provinciaService;

    public ProvinciasController(IProvinciaService provinciaService)
    {
        _provinciaService = provinciaService;
    }

    [HttpGet("by-pais/{paisId}")]
    public async Task<ActionResult<ProvincesListResponseDto>> GetProvinciasByPais(int paisId, [FromQuery] bool includeInactive = false)
    {
        var resultado = await _provinciaService.GetByPaisIdAsync(paisId, includeInactive);
        return resultado.Success ? Ok(resultado) : BadRequest(resultado);
    }
}
