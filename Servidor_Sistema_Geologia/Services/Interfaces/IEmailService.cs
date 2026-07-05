namespace Servidor_Sistema_Geologia.Services.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Envía un correo de solicitud de informe petrográfico al correo destino configurado.
    /// Devuelve true si el envío fue exitoso, false si falló el SMTP (nunca lanza excepción).
    /// </summary>
    Task<bool> EnviarSolicitudInformeAsync(
        string correoSolicitante,
        string? observaciones,
        int elementoId,
        string elementoNombre,
        string elementoCodigo,
        string elementoTipo,
        string? ubicacionDescripcion);
}
