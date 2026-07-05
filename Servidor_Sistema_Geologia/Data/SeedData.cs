using Microsoft.AspNetCore.Identity;

namespace Servidor_Sistema_Geologia.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        // Crear roles si no existen
        await CreateRoleIfNotExists(roleManager, "Admin");
        await CreateRoleIfNotExists(roleManager, "Premium");
        await CreateRoleIfNotExists(roleManager, "Free");

        // Crear usuario administrador por defecto si no existe
        await CreateAdminUserIfNotExists(userManager, configuration);
    }

    private static async Task CreateRoleIfNotExists(RoleManager<IdentityRole<int>> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(roleName));
        }
    }

    private static async Task CreateAdminUserIfNotExists(UserManager<Usuario> userManager, IConfiguration configuration)
    {
        var adminEmail = configuration["DefaultSuperAdmin:Email"] ?? "admin@sistemageologico.com";
        var adminUserName = configuration["DefaultSuperAdmin:UserName"] ?? adminEmail;
        var adminPassword = configuration["DefaultSuperAdmin:Password"] ?? "Admin123!";
        var adminNombre = configuration["DefaultSuperAdmin:NombreCompleto"] ?? "Super Administrador";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new Usuario
            {
                UserName = adminUserName,
                Email = adminEmail,
                NombreCompleto = adminNombre,
                Rol = RolUsuario.Admin,
                FechaCreacion = DateTime.Now,
                EstadoActivo = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}