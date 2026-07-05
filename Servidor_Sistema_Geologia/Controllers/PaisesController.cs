using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.DTO.Locations;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaisesController : ControllerBase
{
    private readonly IPaisService _paisService;

    public PaisesController(IPaisService paisService)
    {
        _paisService = paisService;
    }

    [HttpGet("active")]
    public async Task<ActionResult<CountriesListResponseDto>> GetActivePaises()
    {
        var resultado = await _paisService.GetAllActiveAsync();
        return resultado.Success ? Ok(resultado) : BadRequest(resultado);
    }
}
