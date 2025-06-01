using Microsoft.IdentityModel.Tokens;
using Servidor_Sistema_Geologia.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(Usuario usuario)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.UserName ?? ""),
                new(ClaimTypes.Email, usuario.Email ?? ""),
                new(ClaimTypes.Role, usuario.Rol.ToString()),
                new("NombreCompleto", usuario.NombreCompleto ?? ""),
                new("EstadoActivo", usuario.EstadoActivo.ToString()),
                new("EmailConfirmed", usuario.EmailConfirmed.ToString()),
                new("FechaCreacion", usuario.FechaCreacion.ToString("yyyy-MM-dd")),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                Issuer = GetJwtIssuer(),
                Audience = GetJwtAudience(),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("Token JWT generado exitosamente para usuario: {Email}", usuario.Email);
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar token JWT para usuario: {Email}", usuario.Email);
            throw;
        }
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = GetJwtIssuer(),
                ValidateAudience = true,
                ValidAudience = GetJwtAudience(),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No tolerance for expiry
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // Verificar que es un JWT válido
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Token JWT inválido: algoritmo incorrecto");
                return null;
            }

            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            _logger.LogWarning("Token JWT expirado");
            return null;
        }
        catch (SecurityTokenValidationException ex)
        {
            _logger.LogWarning("Token JWT inválido: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar token JWT");
            return null;
        }
    }



    private string GetJwtSecret()
    {
        var secret = _configuration["JwtSettings:SecretKey"];
        
        if (string.IsNullOrEmpty(secret))
        {
            throw new InvalidOperationException(
                "No se encontró la configuración. " +
                "Problemas en la configuración de JWT.");
        }
        
        if (secret.Length < 32)
        {
            throw new InvalidOperationException(
                "La clave JWT debe tener al menos 32 caracteres para ser segura.");
        }
        
        return secret;
    }

    private string GetJwtIssuer()
    {
        return _configuration["JwtSettings:Issuer"] ?? "SistemaGeologico";
    }

    private string GetJwtAudience()
    {
        return _configuration["JwtSettings:Audience"] ?? "SistemaGeologico-API";
    }

    private int GetTokenExpiryMinutes()
    {
        if (int.TryParse(_configuration["JwtSettings:ExpirationMinutes"], out int minutes))
        {
            return minutes;
        }
        return 720; // 12 horas (720 minutos) por defecto
    }
}