using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using Hephaestus.Api.Data;
using Hephaestus.Api.Models;
using Hephaestus.Api.Options;
using Hephaestus.Api.Security;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hephaestus.Api.Services;

public sealed class GoogleAuthService(
    HephaestusDbContext database,
    IOptions<GoogleOptions> options)
{
    public const string ExternalCookieScheme = "GoogleTemporary";
    public const string GoogleScheme = "Google";
    private static readonly TimeSpan ExternalCodeLifetime = TimeSpan.FromMinutes(1);
    private readonly GoogleOptions settings = options.Value;

    public async Task<User> AuthenticateWebAsync(
        string googleId,
        string email,
        string name,
        CancellationToken cancellationToken) =>
        await FindOrCreateStandardUserAsync(googleId, email, name, cancellationToken);

    public async Task<User?> AuthenticateMobileAsync(
        string idToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.WebClientId))
            throw new InvalidOperationException("O Web Client ID Google não está configurado.");

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [settings.WebClientId]
                });
        }
        catch (InvalidJwtException)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (payload.EmailVerified != true ||
            string.IsNullOrWhiteSpace(payload.Subject) ||
            string.IsNullOrWhiteSpace(payload.Email))
        {
            return null;
        }

        return await FindOrCreateStandardUserAsync(
            payload.Subject,
            payload.Email,
            string.IsNullOrWhiteSpace(payload.Name) ? payload.Email : payload.Name,
            cancellationToken);
    }

    public async Task<string> CreateExternalCodeAsync(
        User user,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var rawCode = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

        database.ExternalLoginCodes.Add(new ExternalLoginCode
        {
            ExternalLoginCodeId = Guid.NewGuid(),
            UsersId = user.UsersId,
            CodeHash = HashCode(rawCode),
            CreatedAt = now,
            ExpiresAt = now.Add(ExternalCodeLifetime)
        });

        await database.SaveChangesAsync(cancellationToken);
        return rawCode;
    }

    public async Task<User?> ExchangeExternalCodeAsync(
        string rawCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rawCode)) return null;

        var codeHash = HashCode(rawCode);
        var code = await database.ExternalLoginCodes
            .Include(item => item.User)
            .ThenInclude(user => user.Role)
            .SingleOrDefaultAsync(item => item.CodeHash == codeHash, cancellationToken);

        var now = DateTime.UtcNow;
        if (code is null || code.UsedAt is not null || code.ExpiresAt <= now ||
            !code.User.IsActive || code.User.Role.Name != RoleNames.Standard)
        {
            return null;
        }

        code.UsedAt = now;
        await database.SaveChangesAsync(cancellationToken);
        return code.User;
    }

    private async Task<User> FindOrCreateStandardUserAsync(
        string googleId,
        string email,
        string name,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await database.Users
            .Include(item => item.Role)
            .SingleOrDefaultAsync(item => item.GoogleAccountId == googleId, cancellationToken);

        user ??= await database.Users
            .Include(item => item.Role)
            .SingleOrDefaultAsync(item => item.Email == normalizedEmail, cancellationToken);

        if (user is not null)
        {
            if (!user.IsActive)
                throw new InvalidOperationException("A conta Hephaestus está inativa.");

            if (user.Role.Name != RoleNames.Standard)
                throw new InvalidOperationException("O login Google está disponível apenas para utilizadores Standard.");

            if (!string.IsNullOrWhiteSpace(user.GoogleAccountId) && user.GoogleAccountId != googleId)
                throw new InvalidOperationException("Este e-mail já está associado a outra conta Google.");

            if (string.IsNullOrWhiteSpace(user.GoogleAccountId))
            {
                user.GoogleAccountId = googleId;
                await database.SaveChangesAsync(cancellationToken);
            }

            return user;
        }

        var standardRole = await database.Roles.SingleOrDefaultAsync(
            role => role.Name == RoleNames.Standard,
            cancellationToken)
            ?? throw new InvalidOperationException("A role Standard não existe na base de dados.");

        user = new User
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            GoogleAccountId = googleId,
            PasswordHash = null,
            RolesId = standardRole.RolesId,
            Role = standardRole,
            IsActive = true,
            Is2FAEnabled = false
        };

        database.Users.Add(user);
        await database.SaveChangesAsync(cancellationToken);
        return user;
    }

    private static string HashCode(string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));
}
