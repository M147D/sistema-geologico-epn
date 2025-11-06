using Servidor_Sistema_Geologia.DTO.Ubicaciones;

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

public class PaginatedElementosGeologicosDto
{
    public List<ElementoGeologicoListDto> ElementosGeologicos { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    
    // Estadísticas adicionales
    public Dictionary<string, int> TipoStats { get; set; } = new();
    public Dictionary<string, int> EstadoStats { get; set; } = new();
}

public class ElementoGeologicoResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ElementoGeologicoDetailDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ElementosGeologicosListResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaginatedElementosGeologicosDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}
