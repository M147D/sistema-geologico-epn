using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Galeria;

namespace Servidor_Sistema_Geologia;

public class SistemaGeologicoDbContext : IdentityDbContext<Usuario, Microsoft.AspNetCore.Identity.IdentityRole<int>, int>
{
	public SistemaGeologicoDbContext(DbContextOptions<SistemaGeologicoDbContext> options)
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

		// Fosil - TipoFosil como entero
		modelBuilder.Entity<Fosil>()
			.Property(f => f.TipoFosil)
			.HasConversion<int>()
			.HasColumnName("tipoFosil");

		// Roca - TipoRoca como entero
		modelBuilder.Entity<Roca>()
			.Property(r => r.TipoRoca)
			.HasConversion<int>()
			.HasColumnName("tipoRoca");

		// Mineral - TipoMineral como entero
		modelBuilder.Entity<Mineral>()
			.Property(m => m.TipoMineral)
			.HasConversion<int>()
			.HasColumnName("tipoMineral");

		// =====================================
		// CONFIGURACIÓN DE ENTIDADES EXISTENTES
		// =====================================

		// Configuración para ElementoGeologico con discriminador para herencia
		modelBuilder.Entity<ElementoGeologico>()
			.HasDiscriminator<string>("tipoElemento")
			.HasValue<Roca>("Roca")
			.HasValue<Fosil>("Fosil")
			.HasValue<Mineral>("Mineral");

		// Relaciones ElementoGeologico
		modelBuilder.Entity<ElementoGeologico>()
			.HasOne(e => e.Ubicacion)
			.WithMany(u => u.ElementosGeologicos)
			.HasForeignKey(e => e.UbicacionId)
			.OnDelete(DeleteBehavior.Restrict);

		// Relación uno-a-uno: ElementoGeologico ← Galería (FK en Galería)
		modelBuilder.Entity<GaleriaElementoGeologico>()
			.HasOne(g => g.ElementoGeologico)
			.WithOne(e => e.Galeria)
			.HasForeignKey<GaleriaElementoGeologico>(g => g.ElementoGeologicoId)
			.OnDelete(DeleteBehavior.Cascade); // Cascade: Si se elimina el elemento, se elimina la galería

		// Configuración para GaleriaElementoGeologico
		modelBuilder.Entity<GaleriaElementoGeologico>()
			.HasMany(g => g.Fotos)
			.WithOne(f => f.Galeria)
			.HasForeignKey(f => f.GaleriaElementosGeologicoId)
			.OnDelete(DeleteBehavior.Cascade);

		// Configuración para Ubicacion
		// Relación directa con País (para casos donde no se conoce la provincia)
		modelBuilder.Entity<Ubicacion>()
			.HasOne(u => u.Pais)
			.WithMany()
			.HasForeignKey(u => u.PaisId)
			.OnDelete(DeleteBehavior.Restrict);

		// Relación con Provincia (opcional, más específica)
		modelBuilder.Entity<Ubicacion>()
			.HasOne(u => u.Provincia)
			.WithMany(p => p.Ubicaciones)
			.HasForeignKey(u => u.ProvinciaId)
			.OnDelete(DeleteBehavior.Restrict);

		// Configuración para Provincia
		modelBuilder.Entity<Provincia>()
			.HasOne(p => p.Pais)
			.WithMany(pa => pa.Provincias)
			.HasForeignKey(p => p.PaisId)
			.OnDelete(DeleteBehavior.Restrict);

		// =====================================
		// CONFIGURACIÓN DE AUDITORÍA (EntidadAuditable)
		// =====================================

		// ElementoGeologico - Relaciones de auditoría
		modelBuilder.Entity<ElementoGeologico>()
			.HasOne(e => e.UsuarioCreacion)
			.WithMany()
			.HasForeignKey(e => e.UsuarioCreacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<ElementoGeologico>()
			.HasOne(e => e.UsuarioActualizacion)
			.WithMany()
			.HasForeignKey(e => e.UsuarioActualizacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<ElementoGeologico>()
			.HasOne(e => e.UsuarioEliminacion)
			.WithMany()
			.HasForeignKey(e => e.UsuarioEliminacionId)
			.OnDelete(DeleteBehavior.Restrict);

		// Pais - Relaciones de auditoría
		modelBuilder.Entity<Pais>()
			.HasOne(p => p.UsuarioCreacion)
			.WithMany()
			.HasForeignKey(p => p.UsuarioCreacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Pais>()
			.HasOne(p => p.UsuarioActualizacion)
			.WithMany()
			.HasForeignKey(p => p.UsuarioActualizacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Pais>()
			.HasOne(p => p.UsuarioEliminacion)
			.WithMany()
			.HasForeignKey(p => p.UsuarioEliminacionId)
			.OnDelete(DeleteBehavior.Restrict);

		// Provincia - Relaciones de auditoría
		modelBuilder.Entity<Provincia>()
			.HasOne(p => p.UsuarioCreacion)
			.WithMany()
			.HasForeignKey(p => p.UsuarioCreacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Provincia>()
			.HasOne(p => p.UsuarioActualizacion)
			.WithMany()
			.HasForeignKey(p => p.UsuarioActualizacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Provincia>()
			.HasOne(p => p.UsuarioEliminacion)
			.WithMany()
			.HasForeignKey(p => p.UsuarioEliminacionId)
			.OnDelete(DeleteBehavior.Restrict);

		// Ubicacion - Relaciones de auditoría
		modelBuilder.Entity<Ubicacion>()
			.HasOne(u => u.UsuarioCreacion)
			.WithMany()
			.HasForeignKey(u => u.UsuarioCreacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Ubicacion>()
			.HasOne(u => u.UsuarioActualizacion)
			.WithMany()
			.HasForeignKey(u => u.UsuarioActualizacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Ubicacion>()
			.HasOne(u => u.UsuarioEliminacion)
			.WithMany()
			.HasForeignKey(u => u.UsuarioEliminacionId)
			.OnDelete(DeleteBehavior.Restrict);

		// GaleriaElementoGeologico - Relaciones de auditoría
		modelBuilder.Entity<GaleriaElementoGeologico>()
			.HasOne(g => g.UsuarioCreacion)
			.WithMany()
			.HasForeignKey(g => g.UsuarioCreacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<GaleriaElementoGeologico>()
			.HasOne(g => g.UsuarioActualizacion)
			.WithMany()
			.HasForeignKey(g => g.UsuarioActualizacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<GaleriaElementoGeologico>()
			.HasOne(g => g.UsuarioEliminacion)
			.WithMany()
			.HasForeignKey(g => g.UsuarioEliminacionId)
			.OnDelete(DeleteBehavior.Restrict);

		// FotoElemento - Relaciones de auditoría
		modelBuilder.Entity<FotoElemento>()
			.HasOne(f => f.UsuarioCreacion)
			.WithMany()
			.HasForeignKey(f => f.UsuarioCreacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<FotoElemento>()
			.HasOne(f => f.UsuarioActualizacion)
			.WithMany()
			.HasForeignKey(f => f.UsuarioActualizacionId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<FotoElemento>()
			.HasOne(f => f.UsuarioEliminacion)
			.WithMany()
			.HasForeignKey(f => f.UsuarioEliminacionId)
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

		// CONFIGURACIÓN DE SOFT DELETE PARA ELEMENTOS GEOLÓGICOS
		modelBuilder.Entity<ElementoGeologico>()
			.Property(e => e.FechaCreacion)
			.IsRequired()
			.HasDefaultValueSql("GETDATE()");

		modelBuilder.Entity<ElementoGeologico>()
			.Property(e => e.EstadoActivo)
			.IsRequired()
			.HasDefaultValue(true);

		modelBuilder.Entity<ElementoGeologico>()
			.Property(e => e.FechaActualizacion)
			.IsRequired(false);

		// Índices para mejor rendimiento
		modelBuilder.Entity<ElementoGeologico>()
			.HasIndex(e => e.Codigo)
			.IsUnique()
			.HasDatabaseName("IX_ElementosGeologicos_Codigo");

		modelBuilder.Entity<Usuario>()
			.HasIndex(u => u.Email)
			.IsUnique()
			.HasDatabaseName("IX_Usuarios_Email");

		// =====================================
		// RENOMBRADO DE TABLAS (prefijo tb_)
		// =====================================

		modelBuilder.Entity<ElementoGeologico>().ToTable("tb_ElementosGeologicos");
		modelBuilder.Entity<Pais>().ToTable("tb_Paises");
		modelBuilder.Entity<Provincia>().ToTable("tb_Provincias");
		modelBuilder.Entity<Ubicacion>().ToTable("tb_Ubicaciones");
		modelBuilder.Entity<GaleriaElementoGeologico>().ToTable("tb_GaleriaElementosGeologicos");
		modelBuilder.Entity<FotoElemento>().ToTable("tb_FotosElementos");

		// =====================================
		// RENOMBRADO DE COLUMNAS (camelCase)
		// =====================================

		// --- ElementoGeologico (incluye propiedades heredadas de EntidadAuditable) ---
		modelBuilder.Entity<ElementoGeologico>(entity =>
		{
			// Propiedades de EntidadAuditable
			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.FechaCreacion).HasColumnName("fechaCreacion");
			entity.Property(e => e.UsuarioCreacionId).HasColumnName("usuarioCreacionId");
			entity.Property(e => e.FechaActualizacion).HasColumnName("fechaActualizacion");
			entity.Property(e => e.UsuarioActualizacionId).HasColumnName("usuarioActualizacionId");
			entity.Property(e => e.FechaEliminacion).HasColumnName("fechaEliminacion");
			entity.Property(e => e.UsuarioEliminacionId).HasColumnName("usuarioEliminacionId");
			entity.Property(e => e.EstadoActivo).HasColumnName("estadoActivo");

			// Propiedades propias de ElementoGeologico
			entity.Property(e => e.Nombre).HasColumnName("nombre");
			entity.Property(e => e.Edad).HasColumnName("edad");
			entity.Property(e => e.Donante).HasColumnName("donante");
			entity.Property(e => e.FechaIngreso).HasColumnName("fechaIngreso");
			entity.Property(e => e.Codigo).HasColumnName("codigo");
			entity.Property(e => e.Ejemplares).HasColumnName("ejemplares");
			entity.Property(e => e.DocumentosRelacionados).HasColumnName("documentosRelacionados");
			entity.Property(e => e.LaminaExiste).HasColumnName("laminaExiste");
			entity.Property(e => e.UbicacionId).HasColumnName("ubicacionId");
		});

		// --- Fosil (columnas específicas) ---
		modelBuilder.Entity<Fosil>(entity =>
		{
			entity.Property(e => e.Especie).HasColumnName("especie");
			entity.Property(e => e.Periodo).HasColumnName("periodo");
		});

		// --- Mineral (columnas específicas) ---
		modelBuilder.Entity<Mineral>(entity =>
		{
			entity.Property(e => e.Litologia).HasColumnName("litologia");
		});

		// --- Roca (columnas específicas) ---
		modelBuilder.Entity<Roca>(entity =>
		{
			entity.Property(e => e.Litologia).HasColumnName("litologia_roca");
		});

		// --- Pais ---
		modelBuilder.Entity<Pais>(entity =>
		{
			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.FechaCreacion).HasColumnName("fechaCreacion");
			entity.Property(e => e.UsuarioCreacionId).HasColumnName("usuarioCreacionId");
			entity.Property(e => e.FechaActualizacion).HasColumnName("fechaActualizacion");
			entity.Property(e => e.UsuarioActualizacionId).HasColumnName("usuarioActualizacionId");
			entity.Property(e => e.FechaEliminacion).HasColumnName("fechaEliminacion");
			entity.Property(e => e.UsuarioEliminacionId).HasColumnName("usuarioEliminacionId");
			entity.Property(e => e.EstadoActivo).HasColumnName("estadoActivo");
			entity.Property(e => e.NombrePais).HasColumnName("nombrePais");
		});

		// --- Provincia ---
		modelBuilder.Entity<Provincia>(entity =>
		{
			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.FechaCreacion).HasColumnName("fechaCreacion");
			entity.Property(e => e.UsuarioCreacionId).HasColumnName("usuarioCreacionId");
			entity.Property(e => e.FechaActualizacion).HasColumnName("fechaActualizacion");
			entity.Property(e => e.UsuarioActualizacionId).HasColumnName("usuarioActualizacionId");
			entity.Property(e => e.FechaEliminacion).HasColumnName("fechaEliminacion");
			entity.Property(e => e.UsuarioEliminacionId).HasColumnName("usuarioEliminacionId");
			entity.Property(e => e.EstadoActivo).HasColumnName("estadoActivo");
			entity.Property(e => e.PaisId).HasColumnName("paisId");
			entity.Property(e => e.NombreProvincia).HasColumnName("nombreProvincia");
		});

		// --- Ubicacion ---
		modelBuilder.Entity<Ubicacion>(entity =>
		{
			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.FechaCreacion).HasColumnName("fechaCreacion");
			entity.Property(e => e.UsuarioCreacionId).HasColumnName("usuarioCreacionId");
			entity.Property(e => e.FechaActualizacion).HasColumnName("fechaActualizacion");
			entity.Property(e => e.UsuarioActualizacionId).HasColumnName("usuarioActualizacionId");
			entity.Property(e => e.FechaEliminacion).HasColumnName("fechaEliminacion");
			entity.Property(e => e.UsuarioEliminacionId).HasColumnName("usuarioEliminacionId");
			entity.Property(e => e.EstadoActivo).HasColumnName("estadoActivo");
			entity.Property(e => e.PaisId).HasColumnName("paisId");
			entity.Property(e => e.ProvinciaId).HasColumnName("provinciaId");
			entity.Property(e => e.Latitud).HasColumnName("latitud");
			entity.Property(e => e.Longitud).HasColumnName("longitud");
			entity.Property(e => e.Localidad).HasColumnName("localidad");
			entity.Property(e => e.Leyenda).HasColumnName("leyenda");
		});

		// --- GaleriaElementoGeologico ---
		modelBuilder.Entity<GaleriaElementoGeologico>(entity =>
		{
			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.FechaCreacion).HasColumnName("fechaCreacion");
			entity.Property(e => e.UsuarioCreacionId).HasColumnName("usuarioCreacionId");
			entity.Property(e => e.FechaActualizacion).HasColumnName("fechaActualizacion");
			entity.Property(e => e.UsuarioActualizacionId).HasColumnName("usuarioActualizacionId");
			entity.Property(e => e.FechaEliminacion).HasColumnName("fechaEliminacion");
			entity.Property(e => e.UsuarioEliminacionId).HasColumnName("usuarioEliminacionId");
			entity.Property(e => e.EstadoActivo).HasColumnName("estadoActivo");
			entity.Property(e => e.ElementoGeologicoId).HasColumnName("elementoGeologicoId");
			entity.Property(e => e.DetalleGrupo).HasColumnName("detalleGrupo");
		});

		// --- FotoElemento ---
		modelBuilder.Entity<FotoElemento>(entity =>
		{
			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.FechaCreacion).HasColumnName("fechaCreacion");
			entity.Property(e => e.UsuarioCreacionId).HasColumnName("usuarioCreacionId");
			entity.Property(e => e.FechaActualizacion).HasColumnName("fechaActualizacion");
			entity.Property(e => e.UsuarioActualizacionId).HasColumnName("usuarioActualizacionId");
			entity.Property(e => e.FechaEliminacion).HasColumnName("fechaEliminacion");
			entity.Property(e => e.UsuarioEliminacionId).HasColumnName("usuarioEliminacionId");
			entity.Property(e => e.EstadoActivo).HasColumnName("estadoActivo");
			entity.Property(e => e.GaleriaElementosGeologicoId).HasColumnName("galeriaElementosGeologicoId");
			entity.Property(e => e.Imagen).HasColumnName("imagen");
			entity.Property(e => e.TipoFoto).HasColumnName("tipoFoto");
			entity.Property(e => e.DescripcionEspecifica).HasColumnName("descripcionEspecifica");
		});

	}
}