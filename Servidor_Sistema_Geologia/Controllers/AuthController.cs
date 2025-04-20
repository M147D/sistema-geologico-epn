using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Constants;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Servidor_Sistema_Geologia.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly GestorSistemaGeologia _context;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AuthController> _logger;

		public AuthController(GestorSistemaGeologia context, IConfiguration configuration, ILogger<AuthController> logger)
		{
			_context = context;
			_configuration = configuration;
			_logger = logger;
		}

		[HttpPost("login/google")]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
		{
			try
			{
				// Verificar configuración de Google
				var clientId = _configuration["Authentication:Google:ClientId"];
				if (string.IsNullOrEmpty(clientId))
				{
					_logger.LogError("El ClientId de Google no está configurado");
					return StatusCode(500, new { message = "Configuración de autenticación incompleta" });
				}

				// Log para depuración
				_logger.LogInformation("Recibido token de Google para validación");

				// Validar token de Google
				var settings = new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new[] { clientId }
				};

				var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, settings);
				_logger.LogInformation("Token validado correctamente para: {Email}", payload.Email);

				// Buscar o crear usuario
				var user = await _context.Usuarios
					.FirstOrDefaultAsync(u => u.Email == payload.Email);

				if (user == null)
				{
					user = new Usuario
					{
						Email = payload.Email,
						NombreUsuario = payload.Name,
						NombreCompleto = payload.GivenName + payload.FamilyName,
						Rol = RolUsuario.Free
					};
					_context.Usuarios.Add(user);
					await _context.SaveChangesAsync();
				}

				// Crear claims principal
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim(ClaimTypes.Name, user.NombreUsuario),
					new Claim(ClaimTypes.Role, user.Rol.ToString())
				};

				var claimsIdentity = new ClaimsIdentity(
					claims,
					CookieAuthenticationDefaults.AuthenticationScheme
				);

				var authProperties = new AuthenticationProperties
				{
					AllowRefresh = true,
					IsPersistent = true,
					ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
				};

				// Iniciar sesión con claims
				await HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimsIdentity),
					authProperties
				);

				Response.Cookies.Append("user_id", user.Id.ToString(), new CookieOptions
				{
					HttpOnly = false,
					Secure = false,
					SameSite = SameSiteMode.Lax,
					Expires = DateTimeOffset.UtcNow.AddHours(7),
					Path = "/"
				});

				return Ok(new
				{
					user = new
					{
						id = user.Id,
						nombre = user.NombreUsuario,
						email = user.Email,
						rol = user.Rol.ToString(),
					},
					message = "Autenticación exitosa"
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error en autenticación: {Message}", ex.Message);
				return StatusCode(500, new { message = "Error de autenticación", details = ex.Message });
			}
		}

		[Authorize]
		[HttpGet("current-user")]
		public IActionResult GetCurrentUser()
		{
			try
			{
				// Obtener claims del usuario autenticado
				var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

				_logger.LogInformation("Verificando usuario actual, autenticado: {IsAuthenticated}", isAuthenticated);

				if (!isAuthenticated)
				{
					return Ok(new { isAuthenticated = false, message = "No autenticado" });
				}

				// Obtener claims del usuario autenticado
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var email = User.FindFirstValue(ClaimTypes.Email);
				var nombre = User.FindFirstValue(ClaimTypes.Name);
				var rol = User.FindFirstValue(ClaimTypes.Role);

				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized(new { message = "No autenticado" });
				}

				return Ok(new
				{
					user = new
					{
						id = int.Parse(userId),
						email,
						nombre,
						rol
					}
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error al obtener usuario actual");
				return StatusCode(500, new { message = "Error interno", details = ex.Message}); 
			}
		}

		// Endpoint para manejar el acceso a páginas protegidas
		[HttpGet("access-denied")]
		public IActionResult AccessDenied()
		{
			return Unauthorized(new { message = "Acceso denegado. No tienes permisos suficientes." });
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			try
			{
				// Sign out from the authentication system
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

				// Delete the user_id cookie with matching attributes
				Response.Cookies.Delete("user_id", new CookieOptions
				{
					HttpOnly = false,
					Secure = false,
					SameSite = SameSiteMode.Lax,
					Path = "/"
				});

				return Ok(new { message = "Cierre de sesión exitoso" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error en cierre de sesión");
				return StatusCode(500, new { message = "Error al cerrar sesión" });
			}
		}

		// Endpoint para verificar si el usuario está autenticado
		[HttpGet("check")]
		public IActionResult CheckAuth()
		{
			bool isAuthenticated = User.Identity?.IsAuthenticated ?? false;

			return Ok(new { isAuthenticated });
		}
	}
}