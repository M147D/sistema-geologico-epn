using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.DTO.Validation;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

/// <summary>
/// DTO para la información de galería con sus fotos.
/// </summary>
public class GaleriaCompletoDto
{
    [StringLength(200)]
    [NoInjection]
    public string DetalleGrupo { get; set; } = "Ninguna";

    public List<FotoCompletoDto> Fotos { get; set; } = new();
}
