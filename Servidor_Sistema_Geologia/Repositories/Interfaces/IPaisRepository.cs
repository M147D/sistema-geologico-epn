namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

public interface IPaisRepository
{
    Task<List<Pais>> GetAllActiveAsync();
}
