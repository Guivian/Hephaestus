
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Hephaestus.Api.Data;
using Hephaestus.Api.DTOs.Authentication;
using Hephaestus.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Hephaestus.Api.Services;

public sealed class TokenService(HephaestusDbContext database, IConfiguration configuration)
{
    public async Task<LoginResponse> CreateSessionAsync(User user, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var accessTokenExpiration = now.AddMinutes(GetPositiveSetting("Jwt:AccessTokenMinutes"));
        var refreshToken = GenerateRefreshToken();

        database.AuthSessions.Add(new AuthSession
        {
            UsersId = user.UsersId,
            RefreshTokenHash = HashToken(refreshToken),
            CreatedAt = now,
            ExpiresAt = now.AddDays(GetPositiveSetting("Jwt:RefreshTokenDays"))
        });

        await database.SaveChangesAsync(cancellationToken);
        return CreateResponse(user, refreshToken, accessTokenExpiration, now);
    }

    public async Task<LoginResponse?> RefreshSessionAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return null;

        var now = DateTime.UtcNow;
        var tokenHash = HashToken(refreshToken);
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);

        var currentSession = await database.AuthSessions
            .Include(session => session.User)
            .ThenInclude(user => user.Role)
            .SingleOrDefaultAsync(session => session.RefreshTokenHash == tokenHash, cancellationToken);

        if (currentSession is null || currentSession.RevokedAt is not null ||
            currentSession.ExpiresAt <= now || !currentSession.User.IsActive)
        {
            return null;
        }

        currentSession.RevokedAt = now;
        var newRefreshToken = GenerateRefreshToken();

        database.AuthSessions.Add(new AuthSession
        {
            UsersId = currentSession.UsersId,
            RefreshTokenHash = HashToken(newRefreshToken),
            CreatedAt = now,
            ExpiresAt = now.AddDays(GetPositiveSetting("Jwt:RefreshTokenDays"))
        });

        await database.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var accessTokenExpiration = now.AddMinutes(GetPositiveSetting("Jwt:AccessTokenMinutes"));
        return CreateResponse(currentSession.User, newRefreshToken, accessTokenExpiration, now);
    }

    public async Task RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return;

        var tokenHash = HashToken(refreshToken);
        var session = await database.AuthSessions
            .SingleOrDefaultAsync(item => item.RefreshTokenHash == tokenHash, cancellationToken);

        if (session is null || session.RevokedAt is not null) return;

        session.RevokedAt = DateTime.UtcNow;
        await database.SaveChangesAsync(cancellationToken);
    }

    private LoginResponse CreateResponse(
        User user, string refreshToken, DateTime accessTokenExpiration, DateTime issuedAt)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UsersId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UsersId.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey())),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            notBefore: issuedAt,
            expires: accessTokenExpiration,
            signingCredentials: credentials);

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token), refreshToken,
            accessTokenExpiration, user.UsersId, user.Name, user.Email, user.Role.Name);
    }

    private string GetJwtKey()
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("A configuração Jwt:Key não existe.");

        return Encoding.UTF8.GetByteCount(key) >= 32
            ? key
            : throw new InvalidOperationException("Jwt:Key deve ter pelo menos 32 bytes.");
    }

    private double GetPositiveSetting(string key)
    {
        var value = configuration.GetValue<double>(key);
        return value > 0
            ? value
            : throw new InvalidOperationException($"A configuração {key} deve ser positiva.");
    }

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
