using Servidor_Sistema_Geologia.DTO.Locations;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class ElementoGeologicoListDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string TipoElemento { get; set; } = string.Empty; // "Fosil", "Mineral", "Roca"
    public string Edad { get; set; } = string.Empty;
    public string Donante { get; set; } = string.Empty;
    public DateTime? FechaIngreso { get; set; }
    public uint? Ejemplares { get; set; }
    public bool LaminaExiste { get; set; }
    public bool EstadoActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    
    // Información de ubicación
    public int? UbicacionId { get; set; } // Para debugging
    public string? Localidad { get; set; }
    public string? NombrePais { get; set; }
    public string? NombreProvincia { get; set; }
    public string? Latitud { get; set; }
    public string? Longitud { get; set; }
    
    // Información específica por tipo
    public string? TipoEspecifico { get; set; } // "Vertebrado", "Silicato", "Ígnea", etc.
    public string? Especie { get; set; } // Solo para fósiles
    public string? Periodo { get; set; } // Solo para fósiles
    public string? Litologia { get; set; } // Para minerales y rocas
    
    // Información de galería
    public int TotalFotos { get; set; }
    public bool TieneGaleria { get; set; }
}
