using Servidor_Sistema_Geologia.DTO.Locations;

namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IProvinciaService
{
    Task<ProvincesListResponseDto> GetByPaisIdAsync(int paisId, bool includeInactive = false);
}
