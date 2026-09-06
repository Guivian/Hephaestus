namespace Hephaestus.Api.Models;

public sealed class ExternalLoginCode
{
    public Guid ExternalLoginCodeId { get; set; }
    public int UsersId { get; set; }

    public required string CodeHash { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }

    public User User { get; set; } = null!;
}
