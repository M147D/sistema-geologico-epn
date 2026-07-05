using Servidor_Sistema_Geologia.DTO.Locations;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class PaisService : IPaisService
{
    private readonly IPaisRepository _paisRepository;
    private readonly ILogger<PaisService> _logger;

    public PaisService(IPaisRepository paisRepository, ILogger<PaisService> logger)
    {
        _paisRepository = paisRepository;
        _logger = logger;
    }

    public async Task<CountriesListResponseDto> GetAllActiveAsync()
    {
        try
        {
            var paises = await _paisRepository.GetAllActiveAsync();

            var paisesList = paises.Select(p => new CountryListDto
            {
                Id = p.Id,
                NombrePais = p.NombrePais,
                FechaCreacion = p.FechaCreacion,
                EstadoActivo = p.EstadoActivo,
                FechaActualizacion = p.FechaActualizacion,
                TotalProvincias = p.Provincias?.Count(pr => pr.EstadoActivo) ?? 0
            }).ToList();

            return new CountriesListResponseDto
            {
                Success = true,
                Message = "Países activos obtenidos exitosamente",
                Data = new PaginatedCountriesDto
                {
                    Paises = paisesList,
                    TotalCount = paisesList.Count,
                    TotalPages = 1,
                    CurrentPage = 1,
                    PageSize = paisesList.Count,
                    HasPrevious = false,
                    HasNext = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener países activos");
            return new CountriesListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al obtener países activos"
            };
        }
    }
}
