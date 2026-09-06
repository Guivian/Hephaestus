using Hephaestus.Api.Data;
using Hephaestus.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Api.Services;

public sealed class AuthenticationService(HephaestusDbContext database, IPasswordHasher<User> passwordHasher)
{
    public async Task<User> RegisterAsync(string name, string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var emailAlreadyExists = await database.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (emailAlreadyExists)
        {
            throw new InvalidOperationException("Já existe uma conta com este e-mail.");
        }

        var standardRole = await database.Roles.SingleOrDefaultAsync(role => role.Name == "Standard", cancellationToken);

        if (standardRole is null)
        {
            throw new InvalidOperationException("A role Standard não existe na base de dados.");
        }

        var user = new User
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            RolesId = standardRole.RolesId,
            IsActive = true,
            Is2FAEnabled = false,
            Role = standardRole
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        database.Users.Add(user);
        await database.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await database.Users.Include(user => user.Role).SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive || string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return null;
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            await database.SaveChangesAsync(cancellationToken);
        }

        return user;
    }
}