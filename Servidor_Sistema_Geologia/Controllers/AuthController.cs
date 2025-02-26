using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Constants;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

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

		// Ruta para procesar el token de Google desde el frontend
		[HttpPost("login/google")]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
		{
			try
			{
				// Verificamos que exista la configuración
				var clientId = _configuration["Authentication:Google:ClientId"];
				if (string.IsNullOrEmpty(clientId))
				{
					_logger.LogError("El ClientId de Google no está configurado");
					return StatusCode(500, new { message = "Configuración de autenticación incompleta. Contacte al administrador." });
				}

				// Validar el token de Google
				var settings = new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new[] { clientId }
				};

				var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, settings);

				// Buscar o crear usuario
				var user = await _context.Usuarios
					.FirstOrDefaultAsync(u => u.Email == payload.Email);

				if (user == null)
				{
					// Nuevo usuario - registrarlo con rol Free por defecto
					user = new Usuario
					{
						Email = payload.Email,
						NombreUsuario = payload.Name,
						Nombres = payload.GivenName,
						Apellidos = payload.FamilyName,
						Rol = RolUsuario.Free
					};
					_context.Usuarios.Add(user);
					await _context.SaveChangesAsync();
					_logger.LogInformation("Nuevo usuario registrado: {Email}", payload.Email);
				}

				// Crear cookie de sesión
				var sessionToken = Guid.NewGuid().ToString(); // Token único para la sesión

				Response.Cookies.Append("session", sessionToken, new CookieOptions
				{
					HttpOnly = true,
					Secure = HttpContext.Request.IsHttps,
					SameSite = SameSiteMode.Lax,
					Expires = DateTimeOffset.UtcNow.AddDays(7)
				});

				// Crear también cookie para almacenar el ID de usuario
				Response.Cookies.Append("user_id", user.Id.ToString(), new CookieOptions
				{
					HttpOnly = false,
					Secure = HttpContext.Request.IsHttps,
					SameSite = SameSiteMode.Lax,
					Expires = DateTimeOffset.UtcNow.AddDays(7)
				});

				// Respuesta exitosa con datos del usuario
				return Ok(new
				{
					user = new
					{
						id = user.Id,
						nombre = user.NombreUsuario,
						email = user.Email,
						rol = user.Rol.ToString(),
						picture = payload.Picture
					},
					message = "Autenticación exitosa"
				});
			}
			catch (InvalidJwtException ex)
			{
				_logger.LogWarning("Token JWT inválido: {Message}", ex.Message);
				return Unauthorized(new { message = "Token inválido o expirado" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error en autenticación de Google: {Message}", ex.Message);
				return StatusCode(500, new { message = "Error de autenticación", error = ex.Message });
			}
		}

		// Endpoint para manejar el acceso a páginas protegidas
		[HttpGet("access-denied")]
		public IActionResult AccessDenied()
		{
			return Unauthorized(new { message = "Acceso denegado. No tienes permisos suficientes." });
		}

		[HttpGet("current-user")]
		public async Task<IActionResult> GetCurrentUser()
		{
			try
			{
				// Verificar si existen las cookies de sesión
				if (!Request.Cookies.TryGetValue("user_id", out var userIdStr) ||
					!Request.Cookies.TryGetValue("session", out var _))
				{
					return Unauthorized(new { message = "No autenticado" });
				}

				if (!int.TryParse(userIdStr, out var userId))
				{
					_logger.LogWarning("Formato de ID de usuario inválido en cookie");
					return Unauthorized(new { message = "Sesión inválida" });
				}

				var user = await _context.Usuarios.FindAsync(userId);

				if (user == null)
				{
					_logger.LogWarning("Usuario {UserId} no encontrado", userId);
					return NotFound(new { message = "Usuario no encontrado" });
				}

				return Ok(new
				{
					user = new
					{
						id = user.Id,
						nombre = user.NombreUsuario,
						email = user.Email,
						rol = user.Rol.ToString()
					}
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error al obtener usuario actual");
				return StatusCode(500, new { message = "Error al obtener información del usuario" });
			}
		}

		[HttpPost("logout")]
		public IActionResult Logout()
		{
			try
			{
				// Eliminar cookies
				Response.Cookies.Delete("session", new CookieOptions
				{
					Secure = HttpContext.Request.IsHttps,
					SameSite = SameSiteMode.Lax
				});

				Response.Cookies.Delete("user_id", new CookieOptions
				{
					Secure = HttpContext.Request.IsHttps,
					SameSite = SameSiteMode.Lax
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
			bool isAuthenticated = Request.Cookies.ContainsKey("session") &&
								  Request.Cookies.ContainsKey("user_id");

			return Ok(new { isAuthenticated });
		}
	}
}