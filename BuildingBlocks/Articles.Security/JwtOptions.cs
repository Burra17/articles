using Microsoft.IdentityModel.Tokens;

namespace Articles.Security;

public class JwtOptions
{
    public required string Issuer { get; init; } 
    public required string Audience { get; init; }
    public required string Secret { get; init; }
    public int ValidForInMinutes { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public TimeSpan ValidFor => TimeSpan.FromMinutes(ValidForInMinutes);

    public DateTime Expiration => IssuedAt.Add(ValidFor);
}
