using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.Models.GIS;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation
{
    public class GeologiaGISRepository : IGeologiaGISRepository
    {
        private readonly PostGISDbContext _context;
        private readonly ILogger<GeologiaGISRepository> _logger;

        public GeologiaGISRepository(
            PostGISDbContext context,
            ILogger<GeologiaGISRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<GeologiaFeature>> GetGeologiaSimplifiedAsync(double tolerance)
        {
            _logger.LogInformation("Obteniendo geología simplificada con tolerancia {Tolerance}", tolerance);

            var features = await _context.Geologia
                .FromSqlRaw(
                    @"SELECT id, ST_Simplify(geom, {0}, true) as geom, cod_a, ""Edad"", ""Litologia""
                      FROM public.tb_geologia
                      WHERE ST_Simplify(geom, {0}, true) IS NOT NULL",
                    tolerance)
                .AsNoTracking()
                .Select(g => new GeologiaFeature
                {
                    Gid = g.Gid,
                    Geom = g.Geom,
                    CodA = g.CodA,
                    Edad = g.Edad,
                    Litologia = g.Litologia
                })
                .ToListAsync();

            _logger.LogInformation("Geometrías simplificadas: {Count} features", features.Count);
            return features;
        }

        public async Task<List<ProvinciaFeature>> GetAllProvinciasAsync()
        {
            _logger.LogInformation("Obteniendo provincias");

            try
            {
                return await _context.Provincias
                    .AsNoTracking()
                    .Select(p => new ProvinciaFeature
                    {
                        Gid = p.Gid,
                        Geom = p.Geom,
                        CodigoProvincia = p.CodigoProvincia,
                        NombreProvincia = p.NombreProvincia,
                        AreaKm2 = p.AreaKm2,
                        Poblacion = p.Poblacion
                    })
                    .ToListAsync();
            }
            catch (Npgsql.PostgresException pgEx) when (pgEx.SqlState == "42P01")
            {
                _logger.LogWarning("Tabla 'provincias' no existe en la base de datos. Retornando lista vacía.");
                return new List<ProvinciaFeature>();
            }
        }

        public async Task<List<EcuadorFeature>> GetAllEcuadorAsync()
        {
            _logger.LogInformation("Obteniendo contorno de Ecuador");

            try
            {
                return await _context.Ecuador
                    .AsNoTracking()
                    .Select(e => new EcuadorFeature
                    {
                        Gid = e.Gid,
                        Geom = e.Geom,
                        NombrePais = e.NombrePais,
                        IsoCode = e.IsoCode,
                        AreaKm2 = e.AreaKm2
                    })
                    .ToListAsync();
            }
            catch (Npgsql.PostgresException pgEx) when (pgEx.SqlState == "42P01")
            {
                _logger.LogWarning("Tabla 'ecuador' no existe en la base de datos. Retornando lista vacía.");
                return new List<EcuadorFeature>();
            }
        }
    }
}
