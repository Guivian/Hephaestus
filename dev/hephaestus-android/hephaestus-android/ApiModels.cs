using System.Text.Json.Serialization;

namespace hephaestus_android;

internal sealed record LoginRequest(string Email, string Password);
internal sealed record TwoFactorRequest(Guid ChallengeId, string Code);
internal sealed record ChallengeRequest(Guid ChallengeId);
internal sealed record GoogleLoginRequest(string IdToken);
internal sealed record RefreshTokenRequest(string RefreshToken);

internal sealed class AuthenticationResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public Guid ChallengeId { get; set; }
    public int ExpiresInSeconds { get; set; }
}

internal sealed class ApiErrorResponse
{
    public string? Message { get; set; }
    public string? Title { get; set; }
}

internal sealed class ApiResult<T>
{
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(AuthenticationResponse))]
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(TwoFactorRequest))]
[JsonSerializable(typeof(ChallengeRequest))]
[JsonSerializable(typeof(GoogleLoginRequest))]
[JsonSerializable(typeof(RefreshTokenRequest))]
[JsonSerializable(typeof(object))]
internal partial class ApiJsonContext : JsonSerializerContext { }
