using System.Xml.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;

namespace Servidor_Sistema_Geologia.Services.Implementation;

/// <summary>
/// Información de una formación geológica desde el QML
/// </summary>
public class GeologiaQmlInfo
{
    public string ColorRgb { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

/// <summary>
/// Servicio para parsear archivos QML de QGIS y extraer mappings de colores y labels
/// </summary>
public class QmlColorService
{
    private readonly ILogger<QmlColorService> _logger;
    private Dictionary<string, GeologiaQmlInfo>? _qmlInfoMap;
    private readonly string _qmlFilePath;

    public QmlColorService(
        ILogger<QmlColorService> logger,
        IConfiguration configuration,
        IWebHostEnvironment env)
    {
        _logger = logger;

        var configured = configuration["QmlColorFilePath"]
            ?? "GIS/Estilo colores geología.qml";

        _qmlFilePath = Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(env.ContentRootPath, configured);
    }

    /// <summary>
    /// Obtiene la información completa (color + label) para un código cod_a
    /// </summary>
    public GeologiaQmlInfo? GetInfoForCodA(string? codA)
    {
        if (string.IsNullOrWhiteSpace(codA))
            return null;

        // Cargar el mapa si no está cargado
        if (_qmlInfoMap == null)
        {
            LoadColorMap();
        }

        return _qmlInfoMap?.GetValueOrDefault(codA);
    }

    /// <summary>
    /// Obtiene solo el color RGB para un código cod_a (retrocompatibilidad)
    /// </summary>
    public string? GetColorForCodA(string? codA)
    {
        return GetInfoForCodA(codA)?.ColorRgb;
    }

    /// <summary>
    /// Carga el mapa de colores desde el archivo QML
    /// </summary>
    private void LoadColorMap()
    {
        try
        {
            _logger.LogInformation("🎨 Cargando mapa de colores desde QML: {Path}", _qmlFilePath);

            if (!File.Exists(_qmlFilePath))
            {
                _logger.LogWarning("⚠️ Archivo QML no encontrado: {Path}", _qmlFilePath);
                _qmlInfoMap = new Dictionary<string, GeologiaQmlInfo>();
                return;
            }

            // Parsear XML
            var doc = XDocument.Load(_qmlFilePath);

            // Diccionario: symbol number → RGB color
            var symbolColors = new Dictionary<string, string>();

            // Extraer colores de símbolos
            var symbols = doc.Descendants("symbol")
                .Where(s => s.Attribute("name")?.Value != "");

            foreach (var symbol in symbols)
            {
                var symbolName = symbol.Attribute("name")?.Value;
                if (string.IsNullOrEmpty(symbolName))
                    continue;

                // Buscar el elemento Option con name="color"
                var colorOption = symbol.Descendants("Option")
                    .FirstOrDefault(o => o.Attribute("name")?.Value == "color");

                if (colorOption != null)
                {
                    var colorValue = colorOption.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(colorValue))
                    {
                        // Extraer RGB del formato: "244,226,196,255,rgb:..."
                        var rgbMatch = Regex.Match(colorValue, @"^(\d+),(\d+),(\d+)");
                        if (rgbMatch.Success)
                        {
                            var r = rgbMatch.Groups[1].Value;
                            var g = rgbMatch.Groups[2].Value;
                            var b = rgbMatch.Groups[3].Value;
                            symbolColors[symbolName] = $"rgb({r}, {g}, {b})";
                        }
                    }
                }
            }

            _logger.LogInformation("📊 Símbolos con colores extraídos: {Count}", symbolColors.Count);

            // Diccionario: cod_a value → GeologiaQmlInfo (color + label)
            _qmlInfoMap = new Dictionary<string, GeologiaQmlInfo>();

            // Extraer categorías (value → symbol → color + label)
            var categories = doc.Descendants("category")
                .Where(c => c.Attribute("value") != null && c.Attribute("symbol") != null);

            foreach (var category in categories)
            {
                var value = category.Attribute("value")?.Value;
                var symbol = category.Attribute("symbol")?.Value;
                var label = category.Attribute("label")?.Value ?? "";

                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(symbol))
                {
                    if (symbolColors.TryGetValue(symbol, out var color))
                    {
                        _qmlInfoMap[value] = new GeologiaQmlInfo
                        {
                            ColorRgb = color,
                            Label = label
                        };
                    }
                }
            }

            _logger.LogInformation("✅ Mapa de colores cargado: {Count} mappings cod_a → RGB + Label", _qmlInfoMap.Count);

            // Log de algunos ejemplos
            var examples = _qmlInfoMap.Take(5);
            foreach (var kvp in examples)
            {
                _logger.LogDebug("   {CodA} → {Color} ({Label})", kvp.Key, kvp.Value.ColorRgb, kvp.Value.Label);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al cargar mapa de colores desde QML");
            _qmlInfoMap = new Dictionary<string, GeologiaQmlInfo>();
        }
    }

    /// <summary>
    /// Obtiene todos los mappings completos (color + label)
    /// </summary>
    public Dictionary<string, GeologiaQmlInfo> GetAllGeologiaInfo()
    {
        if (_qmlInfoMap == null)
        {
            LoadColorMap();
        }

        return _qmlInfoMap ?? new Dictionary<string, GeologiaQmlInfo>();
    }

    /// <summary>
    /// Obtiene todos los mappings de colores (solo colores, retrocompatibilidad)
    /// </summary>
    public Dictionary<string, string> GetAllColorMappings()
    {
        var allInfo = GetAllGeologiaInfo();
        return allInfo.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ColorRgb);
    }
}
