namespace Servidor_Sistema_Geologia.DTO.Locations;

public class ProvinceListDto
{
    public int Id { get; set; }
    public string NombreProvincia { get; set; } = string.Empty;
    public int PaisId { get; set; }
    public string? NombrePais { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoActivo { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public int TotalUbicaciones { get; set; } // Cantidad de ubicaciones en esta provincia
}
