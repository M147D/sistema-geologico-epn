namespace Servidor_Sistema_Geologia.DTO.Locations;

public class CountryListDto
{
    public int Id { get; set; }
    public string NombrePais { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public bool EstadoActivo { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public int TotalProvincias { get; set; } // Cantidad de provincias activas
}
