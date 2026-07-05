using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Servidor_Sistema_Geologia.DTO.GIS;
using Servidor_Sistema_Geologia.Models.GIS;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation
{
    public class GeologiaGISService : IGeologiaGISService
    {
        private readonly IGeologiaGISRepository _repository;
        private readonly QmlColorService _qmlColorService;
        private readonly ILogger<GeologiaGISService> _logger;
        private readonly GeoJsonWriter _geoJsonWriter;

        public GeologiaGISService(
            IGeologiaGISRepository repository,
            QmlColorService qmlColorService,
            ILogger<GeologiaGISService> logger)
        {
            _repository = repository;
            _qmlColorService = qmlColorService;
            _logger = logger;
            _geoJsonWriter = new GeoJsonWriter();
        }

        public async Task<string> GetEcuadorGeoJsonAsync()
        {
            var features = await _repository.GetAllEcuadorAsync();
            _logger.LogInformation("Ecuador: {Count} registros", features.Count);
            var dtos = features.Select(f => (f.Geom, MapToDto(f))).ToList();
            return ConvertToGeoJsonFeatureCollection(dtos);
        }

        public async Task<string> GetProvinciasGeoJsonAsync()
        {
            var features = await _repository.GetAllProvinciasAsync();
            _logger.LogInformation("Provincias: {Count} registros", features.Count);
            var dtos = features.Select(f => (f.Geom, MapToDto(f))).ToList();
            return ConvertToGeoJsonFeatureCollection(dtos);
        }

        public async Task<string> GetGeologiaSimplifiedGeoJsonAsync(double tolerance)
        {
            if (tolerance <= 0 || tolerance > 1)
            {
                _logger.LogWarning("Tolerancia inválida {Tolerance}, usando 0.01", tolerance);
                tolerance = 0.01;
            }

            var features = await _repository.GetGeologiaSimplifiedAsync(tolerance);
            _logger.LogInformation("Geología simplificada (tolerance={Tolerance}): {Count} formaciones", tolerance, features.Count);
            var dtos = features.Select(f => (f.Geom, MapToDto(f))).ToList();
            return ConvertToGeoJsonFeatureCollection(dtos);
        }

        private GeologiaFeatureDto MapToDto(GeologiaFeature entity)
        {
            string? colorRgb = null;
            string? labelQml = null;

            if (!string.IsNullOrEmpty(entity.CodA))
            {
                var qmlInfo = _qmlColorService.GetInfoForCodA(entity.CodA);
                if (qmlInfo != null)
                {
                    colorRgb = qmlInfo.ColorRgb;
                    labelQml = qmlInfo.Label;
                }
            }

            return new GeologiaFeatureDto
            {
                Gid = entity.Gid,
                CodA = entity.CodA,
                Leyenda = entity.Leyenda,
                Edad = entity.Edad,
                Litologia = entity.Litologia,
                ColorRgb = colorRgb,
                LabelQml = labelQml
            };
        }

        private static ProvinciaFeatureDto MapToDto(ProvinciaFeature entity)
        {
            return new ProvinciaFeatureDto
            {
                Gid = entity.Gid,
                CodigoProvincia = entity.CodigoProvincia,
                NombreProvincia = entity.NombreProvincia,
                AreaKm2 = entity.AreaKm2,
                Poblacion = entity.Poblacion
            };
        }

        private static EcuadorFeatureDto MapToDto(EcuadorFeature entity)
        {
            return new EcuadorFeatureDto
            {
                Gid = entity.Gid,
                NombrePais = entity.NombrePais,
                IsoCode = entity.IsoCode,
                AreaKm2 = entity.AreaKm2
            };
        }

        private string ConvertToGeoJsonFeatureCollection<TDto>(List<(Geometry? Geom, TDto Dto)> items) where TDto : class
        {
            var featureCollection = new
            {
                type = "FeatureCollection",
                features = items.Select(item => ConvertToGeoJsonFeature(item.Geom, item.Dto)).ToList()
            };

            return JsonConvert.SerializeObject(featureCollection, Formatting.None);
        }

        private object ConvertToGeoJsonFeature<TDto>(Geometry? geometry, TDto dto) where TDto : class
        {
            var properties = BuildProperties(dto);
            int id = 0;

            if (properties.TryGetValue("Gid", out var gidValue) && gidValue is int gid)
            {
                id = gid;
                properties.Remove("Gid");
            }

            return new
            {
                type = "Feature",
                id,
                geometry = geometry != null
                    ? JsonConvert.DeserializeObject(_geoJsonWriter.Write(geometry))
                    : null,
                properties
            };
        }

        private static Dictionary<string, object?> BuildProperties<TDto>(TDto dto) where TDto : class
        {
            var properties = new Dictionary<string, object?>();

            foreach (var prop in typeof(TDto).GetProperties())
            {
                var value = prop.GetValue(dto);
                if (value != null)
                {
                    properties[prop.Name] = value;
                }
            }

            return properties;
        }
    }
}
