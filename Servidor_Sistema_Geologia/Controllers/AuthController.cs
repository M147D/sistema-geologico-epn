using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Servidor_Sistema_Geologia.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// POST: api/auth/login
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
		{
			// Validación del token de GIS
			try
			{
				var payload = await GoogleJsonWebSignature.ValidateAsync(loginRequest.Email
					, new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new[] { _configuration["Google:ClientId"] }
				});

				// Generar el JWT
				var token = GenerateJwt(payload.Email);
				SetTokenCookie(token);

				return Ok(new { message = "Autenticación exitosa.", token });
			}
			catch (Exception ex)
			{
				return BadRequest($"Error al validar el token: {ex.Message}");
			}
		}

		// POST: api/auth/logout
		[HttpPost("logout")]
		public IActionResult Logout()
		{
			// Invalidar la cookie
			Response.Cookies.Delete("AuthToken");
			return Ok(new { message = "Logout exitoso." });
		}

		// Helper: Generar JWT
		private string GenerateJwt(string email)
		{
			// Implementa la generación del JWT aquí
			return "token_generado";
		}

		// Helper: Configurar la cookie
		private void SetTokenCookie(string token)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.UtcNow.AddHours(1)
			};
			Response.Cookies.Append("AuthToken", token, cookieOptions);
		}
	}

}
