using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Repositories.Implementation;
using Servidor_Sistema_Geologia.Services.Interfaces;
using Servidor_Sistema_Geologia.Services.Implementation;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Registra el DbContext con la cadena de conexion
builder.Services.AddDbContext<Servidor_Sistema_Geologia.GestorSistemaGeologia>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Identity
builder.Services.AddIdentity<Servidor_Sistema_Geologia.Usuario, IdentityRole<int>>(options =>
{
	// Configuración de Password
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
	options.Password.RequiredUniqueChars = 1;

	// Configuración de Lockout
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;

	// Configuración de Usuario
	options.User.AllowedUserNameCharacters =
		"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = true;

	// Configuración de SignIn
	options.SignIn.RequireConfirmedEmail = false;
	options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<Servidor_Sistema_Geologia.GestorSistemaGeologia>()
.AddDefaultTokenProviders();

// ========================================
// CONFIGURACIÓN DE INYECCIÓN DE DEPENDENCIAS
// ========================================

// Registrar Repositorios
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaisRepository, PaisRepository>();
builder.Services.AddScoped<IProvinciaRepository, ProvinciaRepository>();
builder.Services.AddScoped<IElementoGeologicoRepository, ElementoGeologicoRepository>();

// Registrar Repositorios específicos de Elementos Geológicos
builder.Services.AddScoped<IFosilRepository, FosilRepository>();
builder.Services.AddScoped<IMineralRepository, MineralRepository>();
builder.Services.AddScoped<IRocaRepository, RocaRepository>();

// Registrar Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPaisService, PaisService>();
builder.Services.AddScoped<IProvinciaService, ProvinciaService>();
builder.Services.AddScoped<IElementoGeologicoService, ElementoGeologicoService>();

// Registrar Servicios específicos de Elementos Geológicos
builder.Services.AddScoped<IFosilService, FosilService>();
builder.Services.AddScoped<IMineralService, MineralService>();
builder.Services.AddScoped<IRocaService, RocaService>();

// Registrar Repositorios y Servicios de Fotos
builder.Services.AddScoped<IFotoElementoRepository, FotoElementoRepository>();
builder.Services.AddScoped<IFotoElementoService, FotoElementoService>();

// ========================================
// CONFIGURACIÓN DE JWT AUTHENTICATION
// ========================================

// Configurar JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"] ?? "SistemaGeologico2025-SuperSecretKey-DeDesarrollo-NoUsarEnProduccion-MinimumLengthRequired";
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "SistemaGeologico";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "SistemaGeologico-API";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Solo para desarrollo
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero // Sin tolerancia para expiración
    };

    // Configurar eventos para manejo de errores JWT
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"🔥 JWT Authentication failed: {context.Exception.Message}");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = "Token inválido o expirado",
                error = context.Exception?.Message
            }));
        },
        OnTokenValidated = context =>
        {
            var userEmail = context.Principal?.FindFirst("email")?.Value ?? "Unknown";
            Console.WriteLine($"✅ JWT Token validated for user: {userEmail}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"⚠️ JWT Challenge: {context.Error}, {context.ErrorDescription}");
            context.HandleResponse();
            
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Se requiere autenticación",
                    loginRequired = true
                }));
            }
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            Console.WriteLine($"🚫 JWT Forbidden access");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = "Acceso denegado",
                insufficientPermissions = true
            }));
        }
    };
});

// ========================================
// CONFIGURACIÓN DE SWAGGER CON JWT
// ========================================

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sistema Geologia API", Version = "v1" });
	
	// Configurar JWT en Swagger
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer {token}'",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference 
				{ 
					Type = ReferenceType.SecurityScheme, 
					Id = "Bearer" 
				}
			},
			new string[] {}
		}
	});
});

// Configurar CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend",
		policy =>
		{
			policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // URLs del frontend
				  .AllowAnyHeader()
				  .AllowAnyMethod()
				  .AllowCredentials();
		});
});

// Configurar políticas de autorización
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("RequireAdminRole", policy =>
		policy.RequireRole("Admin"));

	options.AddPolicy("RequireAuthenticated", policy =>
		policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// ========================================
// CREAR USUARIO SUPER ADMIN POR DEFECTO
// ========================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<Servidor_Sistema_Geologia.Usuario>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Crear roles si no existen
        var roles = new[] { "Admin", "Premium", "Free" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
                logger.LogInformation("✅ Rol '{Role}' creado exitosamente", role);
            }
        }

        // Obtener configuración del super admin
        var adminEmail = configuration["DefaultSuperAdmin:Email"] ?? "admin@sistemageologico.com";
        var adminUserName = configuration["DefaultSuperAdmin:UserName"] ?? "superadmin";
        var adminPassword = configuration["DefaultSuperAdmin:Password"] ?? "Admin123!";
        var adminNombreCompleto = configuration["DefaultSuperAdmin:NombreCompleto"] ?? "Super Administrador";

        // Verificar si el super admin ya existe
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            // Crear el usuario super admin
            var superAdmin = new Servidor_Sistema_Geologia.Usuario
            {
                UserName = adminUserName,
                Email = adminEmail,
                NombreCompleto = adminNombreCompleto,
                EmailConfirmed = true,
                Rol = Servidor_Sistema_Geologia.RolUsuario.Admin,
                FechaCreacion = DateTime.Now,
                EstadoActivo = true
            };

            var createResult = await userManager.CreateAsync(superAdmin, adminPassword);
            if (createResult.Succeeded)
            {
                // Asignar rol de Admin
                await userManager.AddToRoleAsync(superAdmin, "Admin");
                
                logger.LogInformation("🚀 Super Administrador creado exitosamente:");
                logger.LogInformation("   📧 Email: {Email}", adminEmail);
                logger.LogInformation("   👤 Usuario: {UserName}", adminUserName);
                logger.LogInformation("   🔑 Contraseña: {Password}", adminPassword);
                logger.LogInformation("   🎯 Rol: Admin");
            }
            else
            {
                logger.LogError("❌ Error al crear Super Administrador:");
                foreach (var error in createResult.Errors)
                {
                    logger.LogError("   - {Description}", error.Description);
                }
            }
        }
        else
        {
            logger.LogInformation("ℹ️ El Super Administrador ya existe: {Email}", adminEmail);
            
            // Verificar que tenga el rol de Admin
            if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
            {
                await userManager.AddToRoleAsync(existingAdmin, "Admin");
                logger.LogInformation("✅ Rol Admin asignado al usuario existente: {Email}", adminEmail);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Error al crear el Super Administrador por defecto");
    }
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema Geologia API v1");
		c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
	});

	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/error");
	app.UseHsts();
	app.UseHttpsRedirection();
}

// Aplicar CORS
app.UseCors("AllowFrontend");

// 🔥 ORDEN IMPORTANTE: Authentication ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

// Middleware de diagnóstico JWT
app.Use(async (context, next) =>
{
	var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;
	var userEmail = context.User?.FindFirst("email")?.Value ?? "Anonymous";
	var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
	
	Console.WriteLine($"🔍 Request: {context.Request.Method} {context.Request.Path}");
	Console.WriteLine($"   Auth: {isAuthenticated} | User: {userEmail}");
	Console.WriteLine($"   Token: {(authHeader != null ? "Present" : "Missing")}");
	
	await next();
});

app.MapControllers();

Console.WriteLine("🚀 Sistema Geológico API iniciado con JWT Authentication");
Console.WriteLine("📋 Endpoints disponibles:");
Console.WriteLine("   🔐 POST /api/auth/register - Registrar usuario");
Console.WriteLine("   🔐 POST /api/auth/login - Login (devuelve JWT)");
Console.WriteLine("   👥 GET /api/users - Listar usuarios (requiere token)");
Console.WriteLine("   🌎 GET /api/paises - Listar países");
Console.WriteLine("   🏞️ GET /api/provincias - Listar provincias");
Console.WriteLine("   🗿 GET /api/elementos-geologicos - Listar todos los elementos geológicos");
Console.WriteLine("   🗿 GET /api/elementos-geologicos?tipo=Fosil - Filtrar por tipo");
Console.WriteLine("   🦕 GET /api/elementos-geologicos/fosiles - Listar fósiles");
Console.WriteLine("   💎 GET /api/elementos-geologicos/minerales - Listar minerales");
Console.WriteLine("   🪨 GET /api/elementos-geologicos/rocas - Listar rocas");
Console.WriteLine("   ➕ POST /api/elementos-geologicos/fosiles - Crear fósil");
Console.WriteLine("   ➕ POST /api/elementos-geologicos/minerales - Crear mineral");
Console.WriteLine("   ➕ POST /api/elementos-geologicos/rocas - Crear roca");
Console.WriteLine("   📖 /swagger - Documentación API");

app.Run();