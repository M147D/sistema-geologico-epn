using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Servidor_Sistema_Geologia.Models.GIS
{
    /// <summary>
    /// Entidad que representa el contorno del país Ecuador
    /// Mapea directamente a la tabla 'ecuador' importada desde el shapefile de QGIS
    /// </summary>
    [Table("ecuador", Schema = "public")]
    public class EcuadorFeature
    {
        /// <summary>
        /// ID único de la geometría
        /// </summary>
        [Key]
        [Column("id")]
        public int Gid { get; set; }

        /// <summary>
        /// Geometría espacial del contorno de Ecuador
        /// </summary>
        [Column("geom")]
        public Geometry? Geom { get; set; }

        // ========================================
        // PROPIEDADES DEL SHAPEFILE DE ECUADOR
        // ========================================

        /// <summary>
        /// Nombre del país
        /// </summary>
        [Column("pais")]
        [MaxLength(255)]
        public string? NombrePais { get; set; }

        /// <summary>
        /// Código ISO del país
        /// </summary>
        [Column("iso_code")]
        [MaxLength(3)]
        public string? IsoCode { get; set; }

        /// <summary>
        /// Área total del país en km²
        /// </summary>
        [Column("area_km2")]
        public double? AreaKm2 { get; set; }

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
