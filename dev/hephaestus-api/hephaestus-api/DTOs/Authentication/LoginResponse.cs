
namespace Hephaestus.Api.DTOs.Authentication;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    int UserId,
    string Name,
    string Email,
    string Role);
