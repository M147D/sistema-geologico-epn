using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Servidor_Sistema_Geologia.DTO.Validation;

/// <summary>
/// Coordenada en grados decimales recibida como texto (formulario y Excel la envían así).
/// Vacío = desconocida (null). Acepta punto o coma decimal. Rechaza valores fuera de rango.
/// </summary>
public static class Coordenada
{
    public static bool TryParse(string? texto, double min, double max, out double? valor)
    {
        valor = null;
        if (string.IsNullOrWhiteSpace(texto)) return true;
        var normalizado = texto.Trim().Replace(',', '.');
        if (!double.TryParse(normalizado, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)
            || double.IsNaN(v) || double.IsInfinity(v) || v < min || v > max)
            return false;
        valor = v;
        return true;
    }

    /// <summary>Para valores ya validados por <see cref="CoordenadaAttribute"/>.</summary>
    public static double? Parse(string? texto, double min, double max) =>
        TryParse(texto, min, max, out var v) ? v : throw new ArgumentOutOfRangeException(nameof(texto), texto, "Coordenada inválida");
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class CoordenadaAttribute : ValidationAttribute
{
    public double Min { get; }
    public double Max { get; }

    public CoordenadaAttribute(double min, double max)
    {
        Min = min; Max = max;
        ErrorMessage = $"Debe ser un número entre {min} y {max}, o vacío si se desconoce.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx) =>
        Coordenada.TryParse(value as string, Min, Max, out _)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage, new[] { ctx.MemberName ?? "" });
}
