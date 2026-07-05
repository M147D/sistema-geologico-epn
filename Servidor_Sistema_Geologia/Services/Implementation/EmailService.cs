using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> EnviarSolicitudInformeAsync(
        string correoSolicitante,
        string? observaciones,
        int elementoId,
        string elementoNombre,
        string elementoCodigo,
        string elementoTipo,
        string? ubicacionDescripcion)
    {
        try
        {
            var smtpHost       = _configuration["EmailSettings:SmtpHost"]
                                 ?? throw new InvalidOperationException("EmailSettings:SmtpHost no configurado");
            var smtpPort       = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var useSsl         = bool.Parse(_configuration["EmailSettings:UseSsl"] ?? "false");
            var useStartTls    = bool.Parse(_configuration["EmailSettings:UseStartTls"] ?? "true");
            var senderEmail    = _configuration["EmailSettings:SenderEmail"]
                                 ?? throw new InvalidOperationException("EmailSettings:SenderEmail no configurado");
            var senderName     = _configuration["EmailSettings:SenderName"] ?? "Sistema Geológico";
            var senderPassword = _configuration["EmailSettings:SenderPassword"]
                                 ?? throw new InvalidOperationException("EmailSettings:SenderPassword no configurado");
            var destEmails     = _configuration.GetSection("EmailSettings:DestinationEmails").Get<string[]>();
            if (destEmails == null || destEmails.Length == 0)
                throw new InvalidOperationException("EmailSettings:DestinationEmails no configurado");
            var destName       = _configuration["EmailSettings:DestinationName"] ?? "Petrografía";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            foreach (var email in destEmails)
                message.To.Add(new MailboxAddress(destName, email));
            message.Subject = $"Solicitud de Informe Petrográfico — {elementoTipo} [{elementoCodigo}]";
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = BuildHtmlBody(correoSolicitante, observaciones, elementoId,
                                     elementoNombre, elementoCodigo, elementoTipo, ubicacionDescripcion)
            };

            var secureOption = useSsl
                ? SecureSocketOptions.SslOnConnect
                : (useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, secureOption);
            await client.AuthenticateAsync(senderEmail, senderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation(
                "Solicitud de informe petrográfico enviada. Elemento={Id} ({Codigo}), Solicitante={Email}",
                elementoId, elementoCodigo, correoSolicitante);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error SMTP al enviar solicitud de informe petrográfico para elemento {Id}", elementoId);
            return false;
        }
    }

    private static string BuildHtmlBody(
        string correoSolicitante,
        string? observaciones,
        int elementoId,
        string elementoNombre,
        string elementoCodigo,
        string elementoTipo,
        string? ubicacionDescripcion)
    {
        var obs = string.IsNullOrWhiteSpace(observaciones)
            ? "<em style='color:#9e9e9e'>Sin observaciones</em>"
            : System.Net.WebUtility.HtmlEncode(observaciones).Replace("\n", "<br/>");

        var nombre    = System.Net.WebUtility.HtmlEncode(elementoNombre);
        var codigo    = System.Net.WebUtility.HtmlEncode(elementoCodigo);
        var tipo      = System.Net.WebUtility.HtmlEncode(elementoTipo);
        var ubicacion = System.Net.WebUtility.HtmlEncode(ubicacionDescripcion ?? "No especificada");
        var correo    = System.Net.WebUtility.HtmlEncode(correoSolicitante);
        var fecha     = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        var zona      = DateTime.Now.ToString("zzz");

        // Use $$""" so CSS braces { } don't need escaping; interpolations use {{expr}}
        return $$"""
            <!DOCTYPE html>
            <html lang="es">
            <head>
              <meta charset="utf-8"/>
              <style>
                body { margin:0; padding:0; background:#f4f4f4; font-family:'Segoe UI',Arial,sans-serif; }
                .wrap { max-width:600px; margin:30px auto; background:#fff;
                        border-radius:8px; box-shadow:0 2px 10px rgba(0,0,0,.12); overflow:hidden; }
                .header { background:#1565C0; color:#fff; padding:24px 32px; }
                .header h1 { margin:0 0 4px; font-size:20px; font-weight:700; }
                .header p  { margin:0; font-size:12px; opacity:.8; }
                .body { padding:28px 32px; }
                .section-label { font-size:11px; font-weight:700; color:#1565C0; text-transform:uppercase;
                                 letter-spacing:.06em; margin:24px 0 8px; border-bottom:2px solid #E3F2FD;
                                 padding-bottom:4px; }
                table { width:100%; border-collapse:collapse; }
                td { padding:8px 10px; font-size:14px; vertical-align:top; }
                td.lbl { color:#616161; font-weight:600; width:36%; background:#FAFAFA; }
                tr:nth-child(even) td { background:#F5F5F5; }
                tr:nth-child(even) td.lbl { background:#EEEEEE; }
                .obs { background:#FFFDE7; border-left:4px solid #F9A825;
                       padding:14px 16px; border-radius:4px; font-size:14px; line-height:1.6; }
                .footer { background:#ECEFF1; padding:14px 32px; font-size:11px;
                          color:#90A4AE; text-align:center; }
              </style>
            </head>
            <body>
              <div class="wrap">
                <div class="header">
                  <h1>Solicitud de Informe Petrográfico</h1>
                  <p>Sistema Geológico — generado el {{fecha}} (UTC{{zona}})</p>
                </div>
                <div class="body">
                  <div class="section-label">Elemento geológico</div>
                  <table>
                    <tr><td class="lbl">ID</td>        <td>{{elementoId}}</td></tr>
                    <tr><td class="lbl">Nombre</td>    <td><strong>{{nombre}}</strong></td></tr>
                    <tr><td class="lbl">Código</td>    <td>{{codigo}}</td></tr>
                    <tr><td class="lbl">Tipo</td>      <td>{{tipo}}</td></tr>
                    <tr><td class="lbl">Ubicación</td> <td>{{ubicacion}}</td></tr>
                  </table>

                  <div class="section-label">Datos del solicitante</div>
                  <table>
                    <tr><td class="lbl">Correo</td><td><a href="mailto:{{correo}}">{{correo}}</a></td></tr>
                  </table>

                  <div class="section-label">Observaciones</div>
                  <div class="obs">{{obs}}</div>
                </div>
                <div class="footer">
                  Este mensaje fue generado automáticamente. No responda — contacte directamente al solicitante.
                </div>
              </div>
            </body>
            </html>
            """;
    }
}
