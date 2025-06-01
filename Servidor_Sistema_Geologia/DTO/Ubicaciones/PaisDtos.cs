using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.DTO.Ubicaciones;

// 🌎 DTOS PARA PAÍS

public class CreatePaisDto
{
    [Required(ErrorMessage = "El nombre del país es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre del país no puede exceder 100 caracteres")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "El nombre del país solo puede contener letras y espacios")]
    public string NombrePais { get; set; } = string.Empty;
}

public class UpdatePaisDto
{
    [Required(ErrorMessage = "El nombre del país es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre del país no puede exceder 100 caracteres")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "El nombre del país solo puede contener letras y espacios")]
    public string NombrePais { get; set; } = string.Empty;

    public bool EstadoActivo { get; set; } = true;
}

public class PaisListDto
{
    public int Id { get; set; }
    public string NombrePais { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public bool EstadoActivo { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public int TotalProvincias { get; set; } // Cantidad de provincias activas
}

public class PaisDetailDto
{
    public int Id { get; set; }
    public string NombrePais { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public bool EstadoActivo { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public List<ProvinciaListDto> Provincias { get; set; } = new();
    public int TotalUbicaciones { get; set; } // Total ubicaciones en este país
}

public class PaisFilterDto
{
    public string? NombrePais { get; set; }
    public bool? EstadoActivo { get; set; }
    public DateTime? FechaCreacionDesde { get; set; }
    public DateTime? FechaCreacionHasta { get; set; }
    
    // 🔥 CONTROL DE SOFT DELETE
    /// <summary>
    /// Si es true, incluye países inactivos (eliminados lógicamente) en los resultados.
    /// Por defecto es false (solo países activos).
    /// </summary>
    public bool IncludeInactive { get; set; } = false;
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "NombrePais";
    public bool SortDescending { get; set; } = false;
}

public class PaginatedPaisesDto
{
    public List<PaisListDto> Paises { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}

public class PaisResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaisDetailDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class PaisesListResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaginatedPaisesDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}