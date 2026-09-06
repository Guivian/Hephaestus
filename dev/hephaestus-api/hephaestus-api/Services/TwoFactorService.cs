
using System.Security.Cryptography;
using Hephaestus.Api.Data;
using Hephaestus.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Api.Services;

public sealed class TwoFactorService(
    HephaestusDbContext database,
    IPasswordHasher<TwoFactorChallenge> codeHasher,
    EmailService emailService)
{
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ResendDelay = TimeSpan.FromMinutes(1);
    private const int MaximumAttempts = 5;

    public async Task<Guid> CreateChallengeAsync(User user, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var activeChallenges = await database.TwoFactorChallenges
            .Where(item => item.UsersId == user.UsersId && item.UsedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var activeChallenge in activeChallenges)
            activeChallenge.UsedAt = now;

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var challenge = new TwoFactorChallenge
        {
            TwoFactorChallengeId = Guid.NewGuid(),
            UsersId = user.UsersId,
            CodeHash = string.Empty,
            CreatedAt = now,
            ExpiresAt = now.Add(CodeLifetime),
            FailedAttempts = 0
        };

        challenge.CodeHash = codeHasher.HashPassword(challenge, code);
        database.TwoFactorChallenges.Add(challenge);
        await database.SaveChangesAsync(cancellationToken);

        await emailService.SendTwoFactorCodeAsync(
            user.Email, user.Name, code, cancellationToken);

        return challenge.TwoFactorChallengeId;
    }

    public async Task<User?> VerifyAsync(
        Guid challengeId,
        string code,
        CancellationToken cancellationToken)
    {
        var challenge = await database.TwoFactorChallenges
            .Include(item => item.User)
            .ThenInclude(user => user.Role)
            .SingleOrDefaultAsync(
                item => item.TwoFactorChallengeId == challengeId,
                cancellationToken);

        var now = DateTime.UtcNow;
        if (challenge is null || challenge.UsedAt is not null ||
            challenge.ExpiresAt <= now || challenge.FailedAttempts >= MaximumAttempts ||
            !challenge.User.IsActive)
        {
            return null;
        }

        var result = codeHasher.VerifyHashedPassword(challenge, challenge.CodeHash, code);
        if (result == PasswordVerificationResult.Failed)
        {
            challenge.FailedAttempts++;
            if (challenge.FailedAttempts >= MaximumAttempts)
                challenge.UsedAt = now;

            await database.SaveChangesAsync(cancellationToken);
            return null;
        }

        challenge.UsedAt = now;
        await database.SaveChangesAsync(cancellationToken);
        return challenge.User;
    }

    public async Task<Guid> ResendAsync(Guid challengeId, CancellationToken cancellationToken)
    {
        var previous = await database.TwoFactorChallenges
            .Include(item => item.User)
            .ThenInclude(user => user.Role)
            .SingleOrDefaultAsync(
                item => item.TwoFactorChallengeId == challengeId,
                cancellationToken)
            ?? throw new InvalidOperationException("Desafio 2FA inválido.");

        if (!previous.User.IsActive || previous.UsedAt is not null)
            throw new InvalidOperationException("Desafio 2FA inválido.");

        if (previous.CreatedAt.Add(ResendDelay) > DateTime.UtcNow)
            throw new InvalidOperationException("Aguarde um minuto antes de pedir outro código.");

        return await CreateChallengeAsync(previous.User, cancellationToken);
    }
}
