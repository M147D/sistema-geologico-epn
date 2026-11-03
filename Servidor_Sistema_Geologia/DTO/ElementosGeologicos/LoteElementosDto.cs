using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

/// <summary>Carga masiva de un tipo de elemento (Excel). Se guarda todo o nada.</summary>
public class LoteElementosDto<TCreateDto> where TCreateDto : CreateElementoGeologicoBaseDto
{
    [Required]
    [MinLength(1, ErrorMessage = "El lote no tiene filas")]
    [MaxLength(2000, ErrorMessage = "El lote no puede superar 2000 filas")]
    public List<TCreateDto> Filas { get; set; } = new();
}

public class LoteResultadoDto
{
    public bool Success { get; set; }
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public List<LoteErrorDto> Errores { get; set; } = new();
}

public class LoteErrorDto
{
    /// <summary>Fila del lote, desde 1 (0 = error general).</summary>
    public int Fila { get; set; }
    public string? Codigo { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
