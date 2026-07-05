using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Servidor_Sistema_Geologia.Models.GIS
{
    /// <summary>
    /// Entidad que representa las formaciones geológicas de Ecuador
    /// Mapea directamente a la tabla 'geologia' importada desde el shapefile de QGIS
    /// </summary>
    [Table("geologia", Schema = "public")]
    public class GeologiaFeature
    {
        /// <summary>
        /// ID único de la geometría (generado por PostGIS al importar)
        /// </summary>
        [Key]
        [Column("id")]
        public int Gid { get; set; }

        /// <summary>
        /// Geometría espacial (puede ser MULTIPOLYGON, POLYGON, etc.)
        /// NetTopologySuite maneja las operaciones espaciales
        /// </summary>
        [Column("geom")]
        public Geometry? Geom { get; set; }

        // ========================================
        // PROPIEDADES DEL SHAPEFILE DE GEOLOGÍA
        // ========================================
        // NOTA: Ajustar estos campos según la estructura real
        // de tu shapefile. Ejecutar el script SQL para ver los campos exactos.

        /// <summary>
        /// Leyenda o nombre de la formación geológica (NO MAPEADA A BD)
        /// Se usa solo para compatibilidad con código existente
        /// Usar LabelQml para el nombre desde QML
        /// </summary>
        [NotMapped]
        public string? Leyenda { get; set; }

        /// <summary>
        /// Tipo de litología (tipo de roca)
        /// </summary>
        [Column("Litologia")]
        [MaxLength(500)]
        public string? Litologia { get; set; }

        /// <summary>
        /// Edad geológica de la formación
        /// </summary>
        [Column("Edad")]
        [MaxLength(200)]
        public string? Edad { get; set; }

        /// <summary>
        /// Descripción detallada de la formación - NO MAPEADA
        /// </summary>
        [NotMapped]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Color asignado en QGIS para visualización - NO MAPEADA
        /// </summary>
        [NotMapped]
        public string? Color { get; set; }

        /// <summary>
        /// Código alfanumérico de la formación (usado para colores en QGIS)
        /// </summary>
        [Column("cod_a")]
        [MaxLength(50)]
        public string? CodA { get; set; }

        /// <summary>
        /// Código de la formación geológica - NO MAPEADA
        /// </summary>
        [NotMapped]
        public string? Codigo { get; set; }

        /// <summary>
        /// Área en unidades del sistema de coordenadas - NO MAPEADA
        /// </summary>
        [NotMapped]
        public double? Area { get; set; }

        /// <summary>
        /// Perímetro en unidades del sistema de coordenadas - NO MAPEADA
        /// </summary>
        [NotMapped]
        public double? Perimetro { get; set; }

        // ========================================
        // PROPIEDADES CALCULADAS (NO PERSISTIDAS)
        // ========================================

        /// <summary>
        /// GeoJSON de la geometría (calculado en tiempo de ejecución)
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
        /// Tipo de geometría (POLYGON, MULTIPOLYGON, etc.)
        /// </summary>
        [NotMapped]
        public string? GeometryType => Geom?.GeometryType;

        /// <summary>
        /// SRID (Sistema de Referencia de Coordenadas)
        /// Debe ser 4326 para WGS84 (usado por Leaflet)
        /// </summary>
        [NotMapped]
        public int? SRID => Geom?.SRID;

        /// <summary>
        /// Color RGB calculado desde el archivo QML de QGIS
        /// Formato: "rgb(r, g, b)" - Se asigna en el repositorio
        /// </summary>
        [NotMapped]
        public string? ColorRgb { get; set; }

        /// <summary>
        /// Label (nombre de la formación) desde el archivo QML de QGIS
        /// Ejemplo: "Basalto", "Formación Macuchi", etc.
        /// </summary>
        [NotMapped]
        public string? LabelQml { get; set; }
    }
}
