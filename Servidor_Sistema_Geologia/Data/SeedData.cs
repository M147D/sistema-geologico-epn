using Microsoft.AspNetCore.Identity;

namespace Servidor_Sistema_Geologia.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
        
        // Crear roles si no existen
        await CreateRoleIfNotExists(roleManager, "Admin");
        await CreateRoleIfNotExists(roleManager, "Premium");
        await CreateRoleIfNotExists(roleManager, "Free");
        
        // Crear usuario administrador por defecto si no existe
        await CreateAdminUserIfNotExists(userManager);
    }
    
    private static async Task CreateRoleIfNotExists(RoleManager<IdentityRole<int>> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(roleName));
        }
    }
    
    private static async Task CreateAdminUserIfNotExists(UserManager<Usuario> userManager)
    {
        var adminEmail = "admin@sistema-geologico.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new Usuario
            {
                UserName = adminEmail,
                Email = adminEmail,
                NombreCompleto = "Administrador del Sistema",
                Rol = RolUsuario.Admin,
                FechaCreacion = DateTime.Now,
                EstadoActivo = true,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}