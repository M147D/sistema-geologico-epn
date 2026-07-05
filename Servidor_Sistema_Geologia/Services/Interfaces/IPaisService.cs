using Servidor_Sistema_Geologia.DTO.Locations;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IPaisService
{
    Task<CountriesListResponseDto> GetAllActiveAsync();
}
