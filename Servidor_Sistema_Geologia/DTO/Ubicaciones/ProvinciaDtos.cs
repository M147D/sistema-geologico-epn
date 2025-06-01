using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.DTO.Ubicaciones;

// 🏞️ DTOS PARA PROVINCIA

public class CreateProvinciaDto
{
    [Required(ErrorMessage = "El nombre de la provincia es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre de la provincia no puede exceder 100 caracteres")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "El nombre de la provincia solo puede contener letras y espacios")]
    public string NombreProvincia { get; set; } = string.Empty;

    [Required(ErrorMessage = "El país es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un país válido")]
    public int PaisId { get; set; }
}

public class UpdateProvinciaDto
{
    [Required(ErrorMessage = "El nombre de la provincia es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre de la provincia no puede exceder 100 caracteres")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "El nombre de la provincia solo puede contener letras y espacios")]
    public string NombreProvincia { get; set; } = string.Empty;

    [Required(ErrorMessage = "El país es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un país válido")]
    public int PaisId { get; set; }

    public bool EstadoActivo { get; set; } = true;
}

public class ProvinciaListDto
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

public class ProvinciaDetailDto
{
    public int Id { get; set; }
    public string NombreProvincia { get; set; } = string.Empty;
    public int PaisId { get; set; }
    public string? NombrePais { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstadoActivo { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public int TotalUbicaciones { get; set; }
    // Nota: No incluimos las ubicaciones completas para evitar referencias circulares
    // Si se necesitan, se puede crear un endpoint específico
}

public class ProvinciaFilterDto
{
    public string? NombreProvincia { get; set; }
    public int? PaisId { get; set; }
    public bool? EstadoActivo { get; set; }
    public DateTime? FechaCreacionDesde { get; set; }
    public DateTime? FechaCreacionHasta { get; set; }
    
    // 🔥 CONTROL DE SOFT DELETE
    /// <summary>
    /// Si es true, incluye provincias inactivas (eliminadas lógicamente) en los resultados.
    /// Por defecto es false (solo provincias activas).
    /// </summary>
    public bool IncludeInactive { get; set; } = false;
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "NombreProvincia";
    public bool SortDescending { get; set; } = false;
}

public class PaginatedProvinciasDto
{
    public List<ProvinciaListDto> Provincias { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}

public class ProvinciaResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ProvinciaDetailDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ProvinciasListResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaginatedProvinciasDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}