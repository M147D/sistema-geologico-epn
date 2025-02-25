using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Servidor_Sistema_Geologia.Constants;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DAL;
using Microsoft.EntityFrameworkCore;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly GestorSistemaGeologia _context;

	public AuthController(GestorSistemaGeologia context)
	{
		_context = context;
	}

	[HttpPost("login/google")]
	public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
	{
		try
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);
			var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == payload.Email);

			if (user == null)
			{
				user = new Usuario
				{
					Email = payload.Email,
					NombreUsuario = payload.Name,
					Rol = RolUsuario.Free
				};
				_context.Usuarios.Add(user);
				await _context.SaveChangesAsync();
			}

			// Crear cookie de sesión
			Response.Cookies.Append("session", payload.Email, new CookieOptions
			{
				HttpOnly = true,
				Secure = false,  // Cambiar a true en producción
				SameSite = SameSiteMode.Lax,
				Expires = DateTimeOffset.UtcNow.AddHours(1)
			});

			return Ok(new { user = new { user.NombreUsuario, user.Email, message = "Acceso registrado" } });
		}
		catch (Exception ex)
		{
			return Unauthorized(new { message = "Error de autenticación", error = ex.Message });
		}
	}

	[HttpPost("logout")]
	public IActionResult Logout()
	{
		Response.Cookies.Delete("session", new CookieOptions
		{
			Secure = true,
			SameSite = SameSiteMode.Lax
		});
		return Ok(new { message = "Cierre de sesión exitoso" });
	}
}