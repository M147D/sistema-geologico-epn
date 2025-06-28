namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class ElementoGeologicoFilterDto
{
    // FILTROS BÁSICOS
    public string? Nombre { get; set; }
    public string? Codigo { get; set; }
    public string? Donante { get; set; }
    public string? Edad { get; set; }
    public string? TipoElemento { get; set; } // "Fosil", "Mineral", "Roca"
    
    // FILTROS POR UBICACIÓN
    public int? PaisId { get; set; }
    public int? ProvinciaId { get; set; }
    public int? UbicacionId { get; set; }
    public string? Localidad { get; set; }
    
    // FILTROS POR FECHAS
    public DateTime? FechaIngresoDesde { get; set; }
    public DateTime? FechaIngresoHasta { get; set; }
    public DateTime? FechaCreacionDesde { get; set; }
    public DateTime? FechaCreacionHasta { get; set; }
    
    // FILTROS POR PROPIEDADES ESPECÍFICAS
    public bool? LaminaExiste { get; set; }
    public uint? EjemplaresMin { get; set; }
    public uint? EjemplaresMax { get; set; }
    
    // FILTROS POR ESTADO
    public bool? EstadoActivo { get; set; } = true; // Por defecto solo activos
    public bool IncludeInactive { get; set; } = false;
    
    // FILTROS ESPECÍFICOS POR TIPO
    public SubtipoFosil? TipoFosil { get; set; }
    public SubtipoMineral? TipoMineral { get; set; }
    public SubtipoRoca? TipoRoca { get; set; }
    public string? Especie { get; set; } // Para fósiles
    public string? Periodo { get; set; } // Para fósiles
    public string? Litologia { get; set; } // Para minerales y rocas
    
    // PAGINACIÓN Y ORDENAMIENTO
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "nombre";
    public bool SortDescending { get; set; } = false;
    
    // INCLUSIONES
    public bool IncludeUbicacion { get; set; } = true;
    public bool IncludeGaleria { get; set; } = false;
    public bool IncludeFotos { get; set; } = false;
}
