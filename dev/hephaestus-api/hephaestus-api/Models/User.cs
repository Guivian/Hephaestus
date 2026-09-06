namespace Hephaestus.Api.Models;

public sealed class User
{
    public int UsersId { get; set; }
    public int RolesId { get; set; }
    public int? LocationsId { get; set; }

    public required string Name { get; set; }
    public required string Email { get; set; }

    public string? PasswordHash { get; set; }
    public string? GoogleAccountId { get; set; }

    public bool Is2FAEnabled { get; set; }
    public bool IsActive { get; set; }

    public Role Role { get; set; } = null!;
}