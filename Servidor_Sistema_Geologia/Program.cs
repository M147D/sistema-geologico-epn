using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Infrastructure;
using Servidor_Sistema_Geologia.Models;
using Microsoft.OpenApi.Models;
using Servidor_Sistema_Geologia.Infrastructure.Profiles;
using Servidor_Sistema_Geologia.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Registra el DbContext con la cadena de conexion
builder.Services.AddDbContext<GestorSistemaGeologia>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Swagger con OAuth2
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sistema Geolog�a API", Version = "v1" });
	c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
	{
		Type = SecuritySchemeType.OAuth2,
		Flows = new OpenApiOAuthFlows
		{
			AuthorizationCode = new OpenApiOAuthFlow
			{
				AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
				TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
				Scopes = new Dictionary<string, string>
				{
					{ "openid", "Acceso a la identificaci�n del usuario" },
					{ "profile", "Acceso al perfil del usuario" },
					{ "email", "Acceso al email del usuario" }
				}
			}
		}
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
			},
			new List<string>()
		}
	});
});

// Configurar CORS (una sola pol�tica)
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend",
		policy =>
		{
			policy.WithOrigins("http://localhost:5173") // URL del frontend (Vite)
				  .AllowAnyHeader()
				  .AllowAnyMethod()
				  .AllowCredentials() // Permite cookies
				  .WithExposedHeaders("Set-Cookie");
		});
});

// Verificar configuraci�n de Google
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
var hasGoogleConfig = !string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret);

if (!hasGoogleConfig)
{
	Console.WriteLine("ADVERTENCIA: No se encontr� la configuraci�n de autenticaci�n de Google. " +
					 "Aseg�rate de configurar Authentication:Google:ClientId y Authentication:Google:ClientSecret " +
					 "en appsettings.json o en variables de entorno.");
}

// Configurar autenticaci�n - CORREGIDO: solo una configuraci�n de autenticaci�n
var authBuilder = builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
	// Configuraci�n de la cookie de autenticaci�n
	options.Cookie.Name = "session";
	options.Cookie.HttpOnly = true;
	options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
		? CookieSecurePolicy.SameAsRequest
		: CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.Lax;
	options.Cookie.Path = "/";  // A�ADIDO: Ruta expl�cita para la cookie

	// Configurar expiraci�n y renovaci�n
	options.ExpireTimeSpan = TimeSpan.FromDays(7);
	options.SlidingExpiration = true;

	// Personalizar redirecci�n de login
	options.Events.OnRedirectToLogin = context =>
	{
		context.Response.StatusCode = StatusCodes.Status401Unauthorized;
		context.Response.ContentType = "application/json";
		var result = System.Text.Json.JsonSerializer.Serialize(new
		{
			message = "No autenticado",
			loginRequired = true
		});
		return context.Response.WriteAsync(result);
	};

	// Personalizar manejo de acceso denegado
	options.Events.OnRedirectToAccessDenied = context =>
	{
		context.Response.StatusCode = StatusCodes.Status403Forbidden;
		context.Response.ContentType = "application/json";
		var result = System.Text.Json.JsonSerializer.Serialize(new
		{
			message = "Acceso denegado",
			insufficientPermissions = true
		});
		return context.Response.WriteAsync(result);
	};
});

// Usar el builder de autenticaci�n existente
if (hasGoogleConfig)
{
	authBuilder.AddGoogle(options =>
	{
		options.ClientId = googleClientId!;
		options.ClientSecret = googleClientSecret!;
		options.CallbackPath = "/api/auth/google-response";
		options.Scope.Add("openid");
		options.Scope.Add("profile");
		options.Scope.Add("email");
		options.SaveTokens = true;
	});
}

// Usar pol�ticas de autorizaci�n m�s espec�ficas
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("RequireAdminRole", policy =>
		policy.RequireRole("Admin"));

	options.AddPolicy("RequireAuthenticated", policy =>
		policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Configuro el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();

	// En desarrollo, permitir errores detallados
	app.UseDeveloperExceptionPage();
}
else
{
	// Manejar errores de forma m�s segura en producci�n
	app.UseExceptionHandler("/error");
	app.UseHsts();
	// En producci�n, usar HTTPS
	app.UseHttpsRedirection();
}

// Aplicar CORS
app.UseCors("AllowFrontend");

// Middleware de autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

// Middleware de diagn�stico para depurar problemas de autenticaci�n
app.Use(async (context, next) =>
{
	// Registrar estado de autenticaci�n
	var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;
	Console.WriteLine($"Solicitud a {context.Request.Path} - Autenticado: {isAuthenticated}");
	
	// Continuar con la solicitud
	await next();

	// Registrar cookies en la respuesta
	var cookieHeaders = context.Response.Headers
		.Where(h => h.Key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
		.SelectMany(h => h.Value);

	foreach (var cookie in cookieHeaders)
	{
		Console.WriteLine($"Estableciendo cookie: {cookie}");
	}
});

app.MapControllers();

app.Run();