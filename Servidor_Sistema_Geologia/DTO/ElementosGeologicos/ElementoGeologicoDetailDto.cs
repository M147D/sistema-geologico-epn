using Servidor_Sistema_Geologia.DTO.Locations;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class ElementoGeologicoDetailDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string TipoElemento { get; set; } = string.Empty;
    public string Edad { get; set; } = string.Empty;
    public string Donante { get; set; } = string.Empty;
    public DateTime? FechaIngreso { get; set; }
    public uint? Ejemplares { get; set; }
    public string? DocumentosRelacionados { get; set; }
    public bool LaminaExiste { get; set; }
    public bool EstadoActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public DateTime? FechaEliminacion { get; set; }

    // Auditoría — usuarios responsables
    public string? CreadoPor { get; set; }
    public string? ActualizadoPor { get; set; }
    public string? EliminadoPor { get; set; }

    // Relaciones completas
    public UbicacionDto? Ubicacion { get; set; }
    public GaleriaElementoGeologicoDto? Galeria { get; set; }
    
    // Propiedades específicas por tipo
    public SubtipoFosil? TipoFosil { get; set; }
    public string? Especie { get; set; }
    public string? Periodo { get; set; }
    
    public SubtipoMineral? TipoMineral { get; set; }
    public string? LitologiaMineral { get; set; }
    
    public SubtipoRoca? TipoRoca { get; set; }
    public string? LitologiaRoca { get; set; }
}
