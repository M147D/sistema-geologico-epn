namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IProvinciaRepository
{
    Task<List<Provincia>> GetByPaisIdAsync(int paisId, bool includeInactive = false);
}
