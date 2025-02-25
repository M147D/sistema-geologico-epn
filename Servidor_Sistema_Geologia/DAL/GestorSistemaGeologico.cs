using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.DAL
{
	public class GestorSistemaGeologia : DbContext
	{
		public GestorSistemaGeologia(DbContextOptions<GestorSistemaGeologia> options)
		: base(options)
		{
		}

		public DbSet<ElementoGeologico> ElementosGeologicos { get; set; }
		public DbSet<EstadoElemento> EstadosElementos { get; set; }
		public DbSet<Roca> Rocas { get; set; }
		public DbSet<Fosil> Fosiles { get; set; }
		public DbSet<Pais> Paises { get; set; }
		public DbSet<Provincia> Provincias { get; set; }
		public DbSet<Ubicacion> Ubicaciones { get; set; }
		public DbSet<FotoElemento> FotosElementos { get; set; }
		public DbSet<Usuario> Usuarios { get; set; }
		public DbSet<Acceso> Accesos { get; set; }
		public DbSet<GaleriaElementoGeologico> GaleriaElementosGeologicos { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configuración para Usuario
			modelBuilder.Entity<Usuario>()
				.HasMany(u => u.Accesos)
				.WithOne(a => a.Usuario)
				.HasForeignKey(a => a.UsuarioId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configuración para ElementoGeologico con discriminador para herencia
			modelBuilder.Entity<ElementoGeologico>()
				.HasDiscriminator<string>("TipoElemento")
				.HasValue<Roca>("Roca")
				.HasValue<Fosil>("Fosil");

			modelBuilder.Entity<ElementoGeologico>()
				.HasOne(e => e.Ubicacion)
				.WithMany(u => u.ElementosGeologicos)
				.HasForeignKey(e => e.UbicacionId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ElementoGeologico>()
				.HasOne(e => e.EstadoElemento)
				.WithMany()
				.HasForeignKey(e => e.EstadoElementoId)
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

			// Configuración para Acceso
			modelBuilder.Entity<Acceso>()
				.HasOne(a => a.ElementoGeologico)
				.WithMany()
				.HasForeignKey(a => a.ElementoGeologicoId)
				.OnDelete(DeleteBehavior.Restrict);

			// Enums como strings
			modelBuilder.Entity<EstadoElemento>()
				.Property(e => e.DescripcionEstado)
				.HasConversion<string>();

			modelBuilder.Entity<Usuario>()
				.Property(u => u.Rol)
				.HasConversion<string>();

			modelBuilder.Entity<Acceso>()
				.Property(a => a.Accion)
				.HasConversion<string>();
		}
	}
}