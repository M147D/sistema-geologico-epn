using Servidor_Sistema_Geologia.DTO.Locations;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class ProvinciaService : IProvinciaService
{
    private readonly IProvinciaRepository _provinciaRepository;
    private readonly ILogger<ProvinciaService> _logger;

    public ProvinciaService(IProvinciaRepository provinciaRepository, ILogger<ProvinciaService> logger)
    {
        _provinciaRepository = provinciaRepository;
        _logger = logger;
    }

    public async Task<ProvincesListResponseDto> GetByPaisIdAsync(int paisId, bool includeInactive = false)
    {
        try
        {
            var provincias = await _provinciaRepository.GetByPaisIdAsync(paisId, includeInactive);

            var provinciasList = provincias.Select(p => new ProvinceListDto
            {
                Id = p.Id,
                NombreProvincia = p.NombreProvincia,
                PaisId = p.PaisId,
                NombrePais = p.Pais?.NombrePais,
                FechaCreacion = p.FechaCreacion,
                EstadoActivo = p.EstadoActivo,
                FechaActualizacion = p.FechaActualizacion,
                TotalUbicaciones = p.Ubicaciones?.Count(u => u.EstadoActivo) ?? 0
            }).ToList();

            return new ProvincesListResponseDto
            {
                Success = true,
                Message = "Provincias del país obtenidas exitosamente",
                Data = new PaginatedProvincesDto
                {
                    Provincias = provinciasList,
                    TotalCount = provinciasList.Count,
                    TotalPages = 1,
                    CurrentPage = 1,
                    PageSize = provinciasList.Count,
                    HasPrevious = false,
                    HasNext = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener provincias por país: {PaisId}", paisId);
            return new ProvincesListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor al obtener provincias del país"
            };
        }
    }
}
