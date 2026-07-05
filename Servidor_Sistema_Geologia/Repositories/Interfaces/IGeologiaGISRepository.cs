using Servidor_Sistema_Geologia.Models.GIS;

namespace Servidor_Sistema_Geologia.Repositories.Interfaces
{
    public interface IGeologiaGISRepository
    {
        Task<List<GeologiaFeature>> GetGeologiaSimplifiedAsync(double tolerance);
        Task<List<ProvinciaFeature>> GetAllProvinciasAsync();
        Task<List<EcuadorFeature>> GetAllEcuadorAsync();
    }
}
