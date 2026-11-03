using Microsoft.AspNetCore.Identity;

namespace Servidor_Sistema_Geologia.Data;

/// <summary>
/// Semilla idempotente: superadministrador y filas de referencia ("Desconocido").
/// Se puede ejecutar en cada arranque sin duplicar nada.
///
/// El rol vive solo en la columna Usuarios.Rol, que es lo que viaja en el JWT y usa
/// [Authorize(Roles=...)]. Las tablas de roles de Identity no se usan ni se llenan,
/// para que no haya una segunda copia del rol (REV-01, H-07).
/// </summary>
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services, ILogger logger)
    {
        var context = services.GetRequiredService<SistemaGeologicoDbContext>();
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        var ubicaciones = new Repositories.Implementation.UbicacionRepository(context);
        await ubicaciones.ObtenerProvinciaDesconocidaGlobalAsync();
        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
            logger.LogInformation("País y provincia de referencia 'Desconocido' creados");
        }

        await EnsureSuperAdminAsync(userManager, configuration, logger);
    }

    private static async Task EnsureSuperAdminAsync(
        UserManager<Usuario> userManager, IConfiguration configuration, ILogger logger)
    {
        var email = configuration["DefaultSuperAdmin:Email"];
        var password = configuration["DefaultSuperAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("DefaultSuperAdmin:Email o DefaultSuperAdmin:Password no configurados: no se crea el superadministrador");
            return;
        }

        if (await userManager.FindByEmailAsync(email) != null)
            return; // ya existe: no se toca su rol ni su estado

        var admin = new Usuario
        {
            UserName = configuration["DefaultSuperAdmin:UserName"] ?? "superadmin",
            Email = email,
            NombreCompleto = configuration["DefaultSuperAdmin:NombreCompleto"] ?? "Super Administrador",
            EmailConfirmed = true,
            Rol = RolUsuario.Admin,
            FechaCreacion = DateTime.Now,
            EstadoActivo = true
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                logger.LogError("No se pudo crear el superadministrador: {Description}", error.Description);
            return;
        }

        logger.LogInformation("Superadministrador {Email} creado", email);
    }
}
