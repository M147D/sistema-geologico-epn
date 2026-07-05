using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class SolicitudInformePetrograficoDto
{
    [Required(ErrorMessage = "El correo del solicitante es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
    [StringLength(254)]
    public string CorreoSolicitante { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Las observaciones no pueden exceder los 2000 caracteres")]
    public string? Observaciones { get; set; }
}
