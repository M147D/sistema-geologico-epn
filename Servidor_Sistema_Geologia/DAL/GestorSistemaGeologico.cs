using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.ElementosGeologicos;
using Servidor_Sistema_Geologia.Galeria;

namespace Servidor_Sistema_Geologia;

public class GestorSistemaGeologia : IdentityDbContext<Usuario, Microsoft.AspNetCore.Identity.IdentityRole<int>, int>
{
	public GestorSistemaGeologia(DbContextOptions<GestorSistemaGeologia> options)
	: base(options)
	{
	}

	public DbSet<ElementoGeologico> ElementosGeologicos { get; set; }
	public DbSet<Roca> Rocas { get; set; }
	public DbSet<Fosil> Fosiles { get; set; }
	public DbSet<Mineral> Minerales { get; set; }
	public DbSet<Pais> Paises { get; set; }
	public DbSet<Provincia> Provincias { get; set; }
	public DbSet<Ubicacion> Ubicaciones { get; set; }
	public DbSet<FotoElemento> FotosElementos { get; set; }
	public DbSet<HistorialAcceso> HistorialAccesos { get; set; }
	public DbSet<GaleriaElementoGeologico> GaleriaElementosGeologicos { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// IMPORTANTE: Llamar al base OnModelCreating de Identity PRIMERO
		base.OnModelCreating(modelBuilder);

		// Renombrar las tablas de Identity para que tengan nombres más descriptivos
		modelBuilder.Entity<Usuario>().ToTable("Usuarios");
		modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRole<int>>().ToTable("Roles");
		modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<int>>().ToTable("UsuarioRoles");
		modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<int>>().ToTable("UsuarioClaims");
		modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<int>>().ToTable("UsuarioLogins");
		modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<int>>().ToTable("UsuarioTokens");
		modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>>().ToTable("RolClaims");

		// =====================================
		// CONFIGURACIÓN DE ENUMS
		// =====================================

		// Usuario - Rol como entero (más eficiente y simple)
		modelBuilder.Entity<Usuario>()
			.Property(u => u.Rol)
			.HasConversion<int>()
			.HasColumnName("Rol");

		// HistorialAcceso - Accion como entero
		modelBuilder.Entity<HistorialAcceso>()
			.Property(h => h.Accion)
			.HasConversion<int>()
			.HasColumnName("Accion");

		// Fosil - TipoFosil como entero
		modelBuilder.Entity<Fosil>()
			.Property(f => f.TipoFosil)
			.HasConversion<int>()
			.HasColumnName("TipoFosil");

		// Roca - TipoRoca como entero
		modelBuilder.Entity<Roca>()
			.Property(r => r.TipoRoca)
			.HasConversion<int>()
			.HasColumnName("TipoRoca");

		// Mineral - TipoMineral como entero
		modelBuilder.Entity<Mineral>()
			.Property(m => m.TipoMineral)
			.HasConversion<int>()
			.HasColumnName("TipoMineral");

		// =====================================
		// CONFIGURACIÓN DE ENTIDADES EXISTENTES
		// =====================================

		// Configuración para ElementoGeologico con discriminador para herencia
		modelBuilder.Entity<ElementoGeologico>()
			.HasDiscriminator<string>("TipoElemento")
			.HasValue<Roca>("Roca")
			.HasValue<Fosil>("Fosil")
			.HasValue<Mineral>("Mineral");

		// Relaciones ElementoGeologico
		modelBuilder.Entity<ElementoGeologico>()
			.HasOne(e => e.Ubicacion)
			.WithMany(u => u.ElementosGeologicos)
			.HasForeignKey(e => e.UbicacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<ElementoGeologico>()
			.HasOne(e => e.Galeria)
			.WithOne(g => g.ElementoGeologico)
			.HasForeignKey<ElementoGeologico>(e => e.GaleriaElementosGeologicoId)
			.OnDelete(DeleteBehavior.Restrict);

		// Configuración para GaleriaElementoGeologico
		modelBuilder.Entity<GaleriaElementoGeologico>()
			.HasMany(g => g.Fotos)
			.WithOne(f => f.Galeria)
			.HasForeignKey(f => f.GaleriaElementosGeologicoId)
			.OnDelete(DeleteBehavior.Cascade);

		// Configuración para Ubicacion
		modelBuilder.Entity<Ubicacion>()
			.HasOne(u => u.Provincia)
			.WithMany(p => p.Ubicaciones)
			.HasForeignKey(u => u.ProvinciaId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Ubicacion>()
			.HasOne(u => u.Pais)
			.WithMany(p => p.Ubicaciones)
			.HasForeignKey(u => u.PaisId)
			.OnDelete(DeleteBehavior.Restrict);

		// Configuración para Provincia
		modelBuilder.Entity<Provincia>()
			.HasOne(p => p.Pais)
			.WithMany(pa => pa.Provincias)
			.HasForeignKey(p => p.PaisId)
			.OnDelete(DeleteBehavior.Restrict);

		// Configuración para HistorialAcceso
		modelBuilder.Entity<HistorialAcceso>()
			.HasOne(h => h.Usuario)
			.WithMany()
			.HasForeignKey(h => h.UsuarioId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<HistorialAcceso>()
			.HasOne(h => h.ElementoGeologico)
			.WithMany()
			.HasForeignKey(h => h.ElementoGeologicoId)
			.OnDelete(DeleteBehavior.Restrict);



		// =====================================
		// CONFIGURACIONES ADICIONALES
		// =====================================

		// Configuración de longitudes para campos de Identity
		modelBuilder.Entity<Usuario>()
			.Property(u => u.NombreCompleto)
			.HasMaxLength(200);

		// Configuraciones de campos para mejor rendimiento
		modelBuilder.Entity<ElementoGeologico>()
			.Property(e => e.Nombre)
			.HasMaxLength(200)
			.IsRequired();

		modelBuilder.Entity<ElementoGeologico>()
			.Property(e => e.Codigo)
			.HasMaxLength(100)
			.IsRequired();

		// Índices para mejor rendimiento
		modelBuilder.Entity<ElementoGeologico>()
			.HasIndex(e => e.Codigo)
			.IsUnique()
			.HasDatabaseName("IX_ElementosGeologicos_Codigo");

		modelBuilder.Entity<Usuario>()
			.HasIndex(u => u.Email)
			.IsUnique()
			.HasDatabaseName("IX_Usuarios_Email");
	}
}