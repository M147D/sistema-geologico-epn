namespace Servidor_Sistema_Geologia.DTO.GIS
{
    public class ProvinciaFeatureDto
    {
        public int Gid { get; set; }
        public string? CodigoProvincia { get; set; }
        public string? NombreProvincia { get; set; }
        public double? AreaKm2 { get; set; }
        public int? Poblacion { get; set; }
    }
}
