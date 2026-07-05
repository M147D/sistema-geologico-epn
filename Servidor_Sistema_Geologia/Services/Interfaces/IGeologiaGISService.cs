namespace Servidor_Sistema_Geologia.Services.Interfaces
{
    public interface IGeologiaGISService
    {
        Task<string> GetEcuadorGeoJsonAsync();
        Task<string> GetProvinciasGeoJsonAsync();
        Task<string> GetGeologiaSimplifiedGeoJsonAsync(double tolerance);
    }
}
