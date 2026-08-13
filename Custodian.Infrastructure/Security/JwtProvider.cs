using Custodian.Application.Common.Interfaces;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text;

namespace Custodian.Infrastructure.Security;

public class JwtProvider: IJwtProvider
{
    private readonly JwtSettings _settings;
    public JwtProvider(IOptions<JwtSettings> settings) => _settings = settings.Value;
    public string GenerateToken(Guid UserId, string Email, string Role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, Email),
            new Claim(ClaimTypes.Role, Role),
            new Claim("role", Role)
        };

        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }
}
