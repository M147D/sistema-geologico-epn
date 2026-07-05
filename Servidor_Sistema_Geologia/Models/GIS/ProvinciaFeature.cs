using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Servidor_Sistema_Geologia.Models.GIS
{
    /// <summary>
    /// Entidad que representa las provincias de Ecuador
    /// Mapea directamente a la tabla 'provincias' importada desde el shapefile de QGIS
    /// </summary>
    [Table("provincias", Schema = "public")]
    public class ProvinciaFeature
    {
        /// <summary>
        /// ID único de la geometría
        /// </summary>
        [Key]
        [Column("id")]
        public int Gid { get; set; }

        /// <summary>
        /// Geometría espacial de la provincia
        /// </summary>
        [Column("geom")]
        public Geometry? Geom { get; set; }

        // ========================================
        // PROPIEDADES DEL SHAPEFILE DE PROVINCIAS
        // ========================================
        // NOTA: Ajustar estos campos según tu shapefile real
        // Los nombres comunes en shapefiles de Ecuador incluyen:
        // - dpa_despro (nombre de provincia)
        // - dpa_codpro (código de provincia)

        /// <summary>
        /// Código DPA de la provincia
        /// </summary>
        [Column("dpa_codpro")]
        [MaxLength(10)]
        public string? CodigoProvincia { get; set; }

        /// <summary>
        /// Nombre de la provincia
        /// </summary>
        [Column("dpa_despro")]
        [MaxLength(255)]
        public string? NombreProvincia { get; set; }

        /// <summary>
        /// Área de la provincia en km²
        /// </summary>
        [Column("area_km2")]
        public double? AreaKm2 { get; set; }

        /// <summary>
        /// Población (si está disponible en el shapefile)
        /// </summary>
        [Column("poblacion")]
        public int? Poblacion { get; set; }

        // ========================================
        // PROPIEDADES CALCULADAS
        // ========================================

        /// <summary>
        /// GeoJSON de la geometría
        /// </summary>
        [NotMapped]
        public string? GeoJson
        {
            get
            {
                if (Geom == null) return null;
                var writer = new NetTopologySuite.IO.GeoJsonWriter();
                return writer.Write(Geom);
            }
        }

        /// <summary>
        /// Tipo de geometría
        /// </summary>
        [NotMapped]
        public string? GeometryType => Geom?.GeometryType;

        /// <summary>
        /// SRID (debe ser 4326 para WGS84)
        /// </summary>
        [NotMapped]
        public int? SRID => Geom?.SRID;
    }
}
