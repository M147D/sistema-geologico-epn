namespace Servidor_Sistema_Geologia;

public class UbicacionDto
{
	public int Id { get; set; }
	public string? Latitud { get; set; }
	public string? Longitud { get; set; }
	public string? Localidad { get; set; }
	public string? Leyenda { get; set; }
	public bool EstadoActivo { get; set; } = true;
	public DateTime FechaCreacion { get; set; }
	public DateTime? FechaActualizacion { get; set; }

	// Información de relaciones (para mostrar en listas)
	public string? NombrePais { get; set; }
	public string? NombreProvincia { get; set; }

	// Referencias a IDs para mantener las relaciones
	public int? PaisId { get; set; }
	public int? ProvinciaId { get; set; }
}
