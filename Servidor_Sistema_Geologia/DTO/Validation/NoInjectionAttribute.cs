using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Servidor_Sistema_Geologia.DTO.Validation
{
    /// <summary>
    /// Lista negra defensiva de patrones de inyección SQL/XSS. Las consultas ya están
    /// parametrizadas y la salida se codifica; esto es una capa extra.
    ///
    /// Costo acotado (PEN-12, auditoría 2026-11-03): ASP.NET ejecuta todos los
    /// atributos de una propiedad aunque otro ya haya fallado, así que el tope de
    /// longitud va AQUÍ, antes de cualquier regex. Las regex se compilan una sola vez
    /// con NonBacktracking (tiempo lineal) y además tienen un timeout.
    /// </summary>
    public class NoInjectionAttribute : ValidationAttribute
    {
        public const int MaxLongitud = 5000;

        private static readonly Regex[] _blackList = new[]
        {
            // 1. HTML/XSS: eventos on* (onclick=, onerror=, ...)
            @"\bon\w+\s*=",
            // 2. HTML/XSS: etiquetas de script o cualquier etiqueta
            @"<script[^>]*>|</?[a-zA-Z]+\b[^>]*>",
            // 3. SQL: tautologías (OR 1=1 y similares)
            @"\bOR\b\s+[^=]+=",
            // 4. SQL: palabras clave peligrosas
            @"\b(UNION|SELECT|DELETE|DROP|UPDATE|INSERT|INTO|FROM)\b",
            // 5. SQL: comentarios y apilado
            @"--|/\*|';"
        }.Select(p => new Regex(p,
            RegexOptions.NonBacktracking | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(250))).ToArray();

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string input || input.Length == 0)
                return ValidationResult.Success;

            if (input.Length > MaxLongitud)
                return new ValidationResult($"Entrada demasiado larga (máximo {MaxLongitud} caracteres).");

            try
            {
                foreach (var regex in _blackList)
                {
                    if (regex.IsMatch(input))
                        return new ValidationResult("Entrada no permitida por seguridad.");
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return new ValidationResult("Entrada no permitida por seguridad.");
            }
            return ValidationResult.Success;
        }
    }
}
