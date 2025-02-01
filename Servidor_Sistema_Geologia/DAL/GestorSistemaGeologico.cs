using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.DAL
{
	public class GestorGeologia : DbContext
	{
		public GestorGeologia(DbContextOptions<GestorGeologia> options)
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
	}
}
