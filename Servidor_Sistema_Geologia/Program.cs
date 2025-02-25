using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Servidor_Sistema_Geologia.Application;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Infrastructure;
using Servidor_Sistema_Geologia.Models;
using Microsoft.OpenApi.Models;
using Servidor_Sistema_Geologia.Infrastructure.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Registra el DbContext con la cadena de conexión
builder.Services.AddDbContext<GestorSistemaGeologia>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Swagger con OAuth2
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoogleAuthBackend API", Version = "v1" });
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
					{ "openid", "Acceso a la identificación del usuario" },
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

// Registro de servicios en Program.cs
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Registrar servicios para los diferentes tipos de elementos geológicos
builder.Services.AddScoped<IElementoService<Fosil, FosilDto, FosilDto>, FosilService>();
builder.Services.AddScoped<IElementoService<Roca, RocaDto, RocaDto>, RocaService>();

// Cors
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend",
		policy =>
		{
			policy.WithOrigins("http://localhost:5173") // URL del frontend (Vite)
				  .AllowCredentials() // Permite cookies
				  .AllowAnyHeader()
				  .AllowAnyMethod();
		});
});

// Configurar autenticación con Google y Cookie
builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
	options.Cookie.Name = "AuthCookie";
	options.Cookie.HttpOnly = true;
	options.Cookie.SameSite = SameSiteMode.Lax;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.LoginPath = "/api/auth/google-login";
	options.AccessDeniedPath = "/api/auth/access-denied";
	options.ExpireTimeSpan = TimeSpan.FromHours(2);
	options.SlidingExpiration = true;
})
.AddGoogle(options =>
{
	options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
	options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
	options.CallbackPath = "/api/auth/google-response"; // URI igual Google Cloud Console
	options.Scope.Add("openid");
	options.Scope.Add("profile");
	options.Scope.Add("email");
	options.SaveTokens = true; // Guarda los tokens de autenticación en la cookie
});

builder.Services.AddAuthorization();

// Configurar CORS para desarrollo
builder.Services.AddCors(options =>
{
	options.AddPolicy("DevelopmentCors", builder =>
	{
		builder.WithOrigins("http://localhost:5173") // React frontend
			   .AllowCredentials()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("DevelopmentCors"); // Aplica CORS

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();