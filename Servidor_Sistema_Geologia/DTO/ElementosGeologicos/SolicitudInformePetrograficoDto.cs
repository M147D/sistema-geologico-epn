using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class SolicitudInformePetrograficoDto
{
    // Se ignora: el servidor usa el correo de la sesión (PEN-12). Se conserva para no
    // romper clientes que lo envían.
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
    [StringLength(254)]
    public string? CorreoSolicitante { get; set; }

    [StringLength(2000, ErrorMessage = "Las observaciones no pueden exceder los 2000 caracteres")]
    public string? Observaciones { get; set; }
}
