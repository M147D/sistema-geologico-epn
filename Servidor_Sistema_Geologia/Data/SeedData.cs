using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Servidor_Sistema_Geologia.Data;

/// <summary>
/// Semilla idempotente: roles, superadministrador y filas de referencia
/// ("Desconocido") que el alta de elementos usa cuando no hay país o provincia.
/// Se puede ejecutar en cada arranque sin duplicar nada.
/// </summary>
public static class SeedData
{
    public const string PaisDesconocido = "Desconocido";
    public const string ProvinciaDesconocida = "Desconocida";

    private static readonly string[] Roles = { "Admin", "Premium", "Free", "Invitado" };

    public static async Task InitializeAsync(IServiceProvider services, ILogger logger)
    {
        var context = services.GetRequiredService<SistemaGeologicoDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
                logger.LogInformation("Rol {Role} creado", role);
            }
        }

        await EnsureUbicacionDesconocidaAsync(context, logger);
        await EnsureSuperAdminAsync(userManager, configuration, logger);
    }

    /// <summary>
    /// Devuelve los ids del país "Desconocido" y de su provincia "Desconocida",
    /// creándolos si no existen.
    /// </summary>
    public static async Task<(int PaisId, int ProvinciaId)> EnsureUbicacionDesconocidaAsync(
        SistemaGeologicoDbContext context, ILogger? logger = null)
    {
        var pais = await context.Paises.FirstOrDefaultAsync(p => p.NombrePais == PaisDesconocido);
        if (pais == null)
        {
            pais = new Pais { NombrePais = PaisDesconocido, EstadoActivo = true, FechaCreacion = DateTime.Now };
            context.Paises.Add(pais);
            await context.SaveChangesAsync();
            logger?.LogInformation("País de referencia {Pais} creado con id {Id}", PaisDesconocido, pais.Id);
        }

        var provincia = await context.Provincias
            .FirstOrDefaultAsync(p => p.NombreProvincia == ProvinciaDesconocida && p.PaisId == pais.Id);
        if (provincia == null)
        {
            provincia = new Provincia
            {
                NombreProvincia = ProvinciaDesconocida,
                PaisId = pais.Id,
                EstadoActivo = true,
                FechaCreacion = DateTime.Now
            };
            context.Provincias.Add(provincia);
            await context.SaveChangesAsync();
            logger?.LogInformation("Provincia de referencia {Provincia} creada con id {Id}", ProvinciaDesconocida, provincia.Id);
        }

        return (pais.Id, provincia.Id);
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

        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            if (!await userManager.IsInRoleAsync(existing, "Admin"))
                await userManager.AddToRoleAsync(existing, "Admin");
            return;
        }

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

        await userManager.AddToRoleAsync(admin, "Admin");
        logger.LogInformation("Superadministrador {Email} creado", email);
    }
}
