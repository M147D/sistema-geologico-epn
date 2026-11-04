using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Servidor_Sistema_Geologia;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.Data;
using Servidor_Sistema_Geologia.Repositories.Implementation;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Implementation;
using Servidor_Sistema_Geologia.Services.Interfaces;
using System.Security.Claims;
using System.Text;

// Las columnas de fecha del modelo son "timestamp without time zone" y el código
// usa DateTime.Now (hora local). Este switch mantiene ese comportamiento con Npgsql,
// que por defecto exige DateTime en UTC para timestamptz.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// HEALTHCHECK de Docker sin curl en la imagen: `dotnet <dll> --healthcheck`.
if (args.Contains("--healthcheck"))
{
	var puerto = Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS")?.Split(';', ',')[0] ?? "8080";
	using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
	try
	{
		var respuesta = await http.GetAsync($"http://127.0.0.1:{puerto}/health");
		Environment.Exit(respuesta.IsSuccessStatusCode ? 0 : 1);
	}
	catch
	{
		Environment.Exit(1);
	}
}

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ========================================
// CONFIGURACIÓN OBLIGATORIA (falla al arrancar si falta)
// ========================================

static string Requerido(IConfiguration config, string clave) =>
	string.IsNullOrWhiteSpace(config[clave])
		? throw new InvalidOperationException($"Falta la configuración obligatoria '{clave}' (variable de entorno '{clave.Replace(":", "__")}').")
		: config[clave]!;

var datosConnection = Requerido(configuration, "ConnectionStrings:DefaultConnection");
var gisConnection = Requerido(configuration, "ConnectionStrings:PostGISConnection");
var jwtSecret = Requerido(configuration, "JwtSettings:SecretKey");
if (jwtSecret.Length < 32)
	throw new InvalidOperationException("JwtSettings:SecretKey debe tener al menos 32 caracteres.");
var jwtIssuer = configuration["JwtSettings:Issuer"] ?? "SistemaGeologico";
var jwtAudience = configuration["JwtSettings:Audience"] ?? "SistemaGeologico-API";
var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

// ========================================
// SERVICIOS
// ========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Catálogo e identidad (PostgreSQL)
builder.Services.AddDbContext<Servidor_Sistema_Geologia.SistemaGeologicoDbContext>(options =>
	options.UseNpgsql(datosConnection));

// Capas GIS (PostgreSQL + PostGIS, solo lectura)
builder.Services.AddDbContext<PostGISDbContext>(options =>
	options.UseNpgsql(gisConnection, npgsqlOptions => npgsqlOptions.UseNetTopologySuite()));

builder.Services.AddHealthChecks()
	.AddDbContextCheck<Servidor_Sistema_Geologia.SistemaGeologicoDbContext>("datos")
	.AddDbContextCheck<PostGISDbContext>("gis");

// Identity
builder.Services.AddIdentity<Servidor_Sistema_Geologia.Usuario, IdentityRole<int>>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
	options.Password.RequiredUniqueChars = 1;

	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	// 10 y no 3: con el límite por IP del login, un tercero ya no bloquea una cuenta
	// con tres peticiones (PEN-12). Sigue acotando la fuerza bruta distribuida.
	options.Lockout.MaxFailedAccessAttempts = 10;
	options.Lockout.AllowedForNewUsers = true;

	options.User.AllowedUserNameCharacters =
		"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = true;

	options.SignIn.RequireConfirmedEmail = false;
	options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<Servidor_Sistema_Geologia.SistemaGeologicoDbContext>()
.AddDefaultTokenProviders();

// Repositorios
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaisRepository, PaisRepository>();
builder.Services.AddScoped<IProvinciaRepository, ProvinciaRepository>();
builder.Services.AddScoped<IElementoGeologicoRepository, ElementoGeologicoRepository>();
builder.Services.AddScoped<IFosilRepository, FosilRepository>();
builder.Services.AddScoped<IMineralRepository, MineralRepository>();
builder.Services.AddScoped<IRocaRepository, RocaRepository>();
builder.Services.AddScoped<IFotoElementoRepository, FotoElementoRepository>();
builder.Services.AddScoped<IGeologiaGISRepository, GeologiaGISRepository>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPaisService, PaisService>();
builder.Services.AddScoped<IProvinciaService, ProvinciaService>();
builder.Services.AddScoped<IElementoGeologicoService, ElementoGeologicoService>();
builder.Services.AddScoped<IFosilService, FosilService>();
builder.Services.AddScoped<IMineralService, MineralService>();
builder.Services.AddScoped<IRocaService, RocaService>();
builder.Services.AddScoped<IFotoElementoService, FotoElementoService>();
builder.Services.AddScoped<IGeologiaGISService, GeologiaGISService>();
builder.Services.AddSingleton<QmlColorService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IImageProcessingService, ImageProcessingService>();

// Caché en memoria para imágenes procesadas (miniaturas y marcas de agua)
builder.Services.AddMemoryCache(options =>
{
	options.SizeLimit = 500 * 1024 * 1024; // 500 MB
});

// ========================================
// JWT
// ========================================

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.SaveToken = true;
	// TLS lo termina el proxy (Caddy); la API recibe HTTP dentro de la red de Docker.
	options.RequireHttpsMetadata = false;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtIssuer,
		ValidAudience = jwtAudience,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
		ClockSkew = TimeSpan.Zero
	};

	options.Events = new JwtBearerEvents
	{
		// Solo registra: la respuesta 401 la escribe OnChallenge. Escribir aquí dejaba que
		// las acciones anónimas siguieran con la respuesta ya iniciada.
		OnAuthenticationFailed = context =>
		{
			var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
			logger.LogWarning("JWT inválido: {Message}", context.Exception.Message);
			return Task.CompletedTask;
		},
		// Revalidación por petición (PEN-12): el token vale solo mientras el usuario siga
		// activo, con el mismo rol y el mismo sello de seguridad que cuando se emitió.
		// Desactivar, degradar, cambiar la contraseña o cerrar sesión lo invalida al instante.
		OnTokenValidated = async context =>
		{
			var principal = context.Principal!;
			var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<Servidor_Sistema_Geologia.Usuario>>();
			var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
			var usuario = id == null ? null : await userManager.FindByIdAsync(id);
			if (usuario == null
				|| !usuario.EstadoActivo
				|| principal.FindFirstValue(ClaimTypes.Role) != usuario.Rol.ToString()
				|| principal.FindFirstValue(JwtService.SecurityStampClaim) != usuario.SecurityStamp)
			{
				context.Fail("Sesión revocada");
			}
		},
		OnChallenge = context =>
		{
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
// SWAGGER (solo Development)
// ========================================

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sistema Geologia API", Version = "v1" });
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT en la cabecera Authorization. Pegar solo el token.",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});
	c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
	{
		[new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
	});
});

// ========================================
// CORS: solo hace falta si el frontend se sirve desde otro origen.
// Con el proxy (Vite en desarrollo, Caddy en producción) todo es mismo origen.
// ========================================

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		if (corsOrigins.Length > 0)
			policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
	});
});

// Límites de frecuencia (PEN-12). Responden 429 sin hacer el trabajo.
builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	static string Ip(HttpContext ctx) => ctx.Connection.RemoteIpAddress?.ToString() ?? "sin-ip";

	// Login y registro: por IP del cliente.
	options.AddPolicy(PoliticasLimite.Auth, ctx => RateLimitPartition.GetFixedWindowLimiter(
		"auth:" + Ip(ctx), _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));

	// Solicitud de informe por correo: por usuario...
	options.AddPolicy(PoliticasLimite.Informe, ctx => RateLimitPartition.GetFixedWindowLimiter(
		"informe:" + (ctx.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Ip(ctx)),
		_ => new FixedWindowRateLimiterOptions { PermitLimit = 3, Window = TimeSpan.FromHours(1), QueueLimit = 0 }));

	// ...y en total, para que muchas cuentas autorregistradas no inunden el correo.
	options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
		ctx.Request.Path.Value?.EndsWith("/solicitar-informe", StringComparison.OrdinalIgnoreCase) == true
			? RateLimitPartition.GetFixedWindowLimiter("informe-global", _ => new FixedWindowRateLimiterOptions { PermitLimit = 50, Window = TimeSpan.FromHours(1), QueueLimit = 0 })
			: RateLimitPartition.GetNoLimiter("sin-limite"));
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
	options.AddPolicy("RequireAuthenticated", policy => policy.RequireAuthenticatedUser());
	options.AddPolicy("CanCreateEdit", policy => policy.RequireRole("Admin", "Invitado"));
});

// Cabeceras X-Forwarded-* del proxy inverso (Caddy) dentro de la red de Docker.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
	// Solo se confía en proxys de redes privadas (Caddy dentro de la red de Docker), y
	// solo en el último salto: un cliente no puede falsificar su IP (PEN-12). Caddy
	// reemplaza el X-Forwarded-For que manda el cliente por la IP real.
	options.KnownIPNetworks.Clear();
	options.KnownProxies.Clear();
	options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse("10.0.0.0/8"));
	options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse("172.16.0.0/12"));
	options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse("192.168.0.0/16"));
	options.ForwardLimit = 1;
});

var app = builder.Build();

// ========================================
// MIGRACIONES: paso explícito, no en cada arranque.
//   dotnet Servidor_Sistema_Geologia.dll --migrate
// ========================================

if (args.Contains("--migrate"))
{
	using var scope = app.Services.CreateScope();
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
	var db = scope.ServiceProvider.GetRequiredService<Servidor_Sistema_Geologia.SistemaGeologicoDbContext>();
	logger.LogInformation("Aplicando migraciones del catálogo...");
	await db.Database.MigrateAsync();
	await SeedData.InitializeAsync(scope.ServiceProvider, logger);
	logger.LogInformation("Migraciones y semilla aplicadas.");
	return;
}

// Semilla idempotente al arrancar (roles, superadmin, filas "Desconocido").
using (var scope = app.Services.CreateScope())
{
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
	try
	{
		await SeedData.InitializeAsync(scope.ServiceProvider, logger);
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "No se pudo aplicar la semilla. ¿Faltan las migraciones? Ejecutar con --migrate.");
	}
}

// ========================================
// PIPELINE
// ========================================

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema Geologia API v1");
		c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
	});
}
else
{
	// Respuesta genérica, sin detalles internos.
	app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
	{
		context.Response.StatusCode = StatusCodes.Status500InternalServerError;
		context.Response.ContentType = "application/json";
		await context.Response.WriteAsync("{\"success\":false,\"message\":\"Error interno del servidor\"}");
	}));
	// La redirección a HTTPS la hace Caddy; HSTS se envía cuando la petición llegó por HTTPS.
	app.UseHsts();
}

app.UseCors("AllowFrontend");

app.Use(async (context, next) =>
{
	context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
	context.Response.Headers.Append("X-Frame-Options", "DENY");
	context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
	if (!context.Request.Path.StartsWithSegments("/swagger"))
	{
		context.Response.Headers.Append(
			"Content-Security-Policy",
			"default-src 'none'; frame-ancestors 'none';");
	}
	await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapHealthChecks("/health").AllowAnonymous();
app.MapControllers();

app.Run();
