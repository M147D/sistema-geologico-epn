using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Models.GIS;

namespace Servidor_Sistema_Geologia.DAL
{
    /// <summary>
    /// Contexto de base de datos para PostGIS (PostgreSQL con extensión espacial)
    /// Maneja las entidades geoespaciales importadas desde QGIS
    /// </summary>
    public class PostGISDbContext : DbContext
    {
        public PostGISDbContext(DbContextOptions<PostGISDbContext> options) : base(options)
        {
        }

        // ========================================
        // DBSETS DE ENTIDADES GEOESPACIALES
        // ========================================

        /// <summary>
        /// Formaciones geológicas de Ecuador
        /// </summary>
        public DbSet<GeologiaFeature> Geologia { get; set; } = null!;

        /// <summary>
        /// Provincias de Ecuador
        /// </summary>
        public DbSet<ProvinciaFeature> Provincias { get; set; } = null!;

        /// <summary>
        /// Contorno del país Ecuador
        /// </summary>
        public DbSet<EcuadorFeature> Ecuador { get; set; } = null!;

        // ========================================
        // CONFIGURACIÓN DEL MODELO
        // ========================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Habilitar extensión PostGIS
            modelBuilder.HasPostgresExtension("postgis");

            // ========================================
            // CONFIGURACIÓN DE GEOLOGÍA
            // ========================================
            modelBuilder.Entity<GeologiaFeature>(entity =>
            {
                entity.ToTable("tb_geologia", "public");
                entity.HasKey(e => e.Gid);

                // Configurar columna de geometría
                entity.Property(e => e.Geom)
                    .HasColumnType("geometry")
                    .HasColumnName("geom");

                // Crear índice espacial (si no existe)
                entity.HasIndex(e => e.Geom)
                    .HasMethod("gist")
                    .HasDatabaseName("idx_geologia_geom");

                // Configurar otras propiedades según sea necesario
                // NOTA: Estas propiedades no existen en la tabla geologia de PostGIS
                // Se marcaron como [NotMapped] en el modelo
                // entity.Property(e => e.Leyenda).HasColumnName("leyenda");
                // entity.Property(e => e.Litologia).HasColumnName("litología");
                // entity.Property(e => e.Edad).HasColumnName("edad");
                // entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                // entity.Property(e => e.Color).HasColumnName("color");
                // entity.Property(e => e.Codigo).HasColumnName("codigo");
                // entity.Property(e => e.Area).HasColumnName("area");
                // entity.Property(e => e.Perimetro).HasColumnName("perimetro");
            });

            // ========================================
            // CONFIGURACIÓN DE PROVINCIAS
            // ========================================
            modelBuilder.Entity<ProvinciaFeature>(entity =>
            {
                entity.ToTable("tb_provincias", "public");
                entity.HasKey(e => e.Gid);

                // Configurar columna de geometría
                entity.Property(e => e.Geom)
                    .HasColumnType("geometry")
                    .HasColumnName("geom");

                // Crear índice espacial
                entity.HasIndex(e => e.Geom)
                    .HasMethod("gist")
                    .HasDatabaseName("idx_provincias_geom");

                // Configurar otras propiedades
                entity.Property(e => e.CodigoProvincia).HasColumnName("dpa_codpro");
                entity.Property(e => e.NombreProvincia).HasColumnName("dpa_despro");
                entity.Property(e => e.AreaKm2).HasColumnName("area_km2");
                entity.Property(e => e.Poblacion).HasColumnName("poblacion");
            });

            // ========================================
            // CONFIGURACIÓN DE ECUADOR
            // ========================================
            modelBuilder.Entity<EcuadorFeature>(entity =>
            {
                entity.ToTable("tb_ecuador", "public");
                entity.HasKey(e => e.Gid);

                // Configurar columna de geometría
                entity.Property(e => e.Geom)
                    .HasColumnType("geometry")
                    .HasColumnName("geom");

                // Crear índice espacial
                entity.HasIndex(e => e.Geom)
                    .HasMethod("gist")
                    .HasDatabaseName("idx_ecuador_geom");

                // Configurar otras propiedades
                entity.Property(e => e.NombrePais).HasColumnName("pais");
                entity.Property(e => e.IsoCode).HasColumnName("iso_code");
                entity.Property(e => e.AreaKm2).HasColumnName("area_km2");
            });
        }

        // ========================================
        // MÉTODOS DE DIAGNÓSTICO
        // ========================================

        /// <summary>
        /// Verifica la conexión a PostGIS y la extensión
        /// </summary>
        public async Task<bool> VerifyPostGISConnection()
        {
            try
            {
                var version = await Database.ExecuteSqlRawAsync("SELECT PostGIS_version();");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
