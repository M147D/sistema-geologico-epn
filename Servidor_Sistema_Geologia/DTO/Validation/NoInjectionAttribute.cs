using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Servidor_Sistema_Geologia.DTO.Validation
{
    public class NoInjectionAttribute : ValidationAttribute
    {
        // Lista de patrones sospechosos para prevenir inyecciones SQL y XSS
        private static readonly string[] _blackList =
        {
        // 1. HTML/XSS: Eventos que empiezan con 'on' (onclick, onerror, etc.)
        @"(?i)\bon\w+\s*=", 
        
        // 2. HTML/XSS: Etiquetas de script o cualquier etiqueta sospechosa
        @"(?i)<script.*?>|</?([a-zA-Z]+)\b.*?>", 
        
        // 3. SQL: Tautologías clásicas (OR 1=1 o similares)
        @"(?i)\bOR\b\s+.+=\s*.+", 
        
        // 4. SQL: Comandos peligrosos seguidos de espacio o fin de palabra
        @"(?i)\b(UNION|SELECT|DELETE|DROP|UPDATE|INSERT|INTO|FROM)\b",
        
        // 5. SQL: Comentarios y ejecución de comandos apilados
        @"--|/\*|';"
    };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string input && !string.IsNullOrEmpty(input))
            {
                foreach (var pattern in _blackList)
                {
                    if (Regex.IsMatch(input, pattern))
                    {
                        return new ValidationResult($"Entrada no permitida por seguridad.");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}