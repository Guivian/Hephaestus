using System;

namespace hephaestus_web
{
    public sealed class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public Guid ChallengeId { get; set; }
        public int ExpiresInSeconds { get; set; }
    }

    public sealed class ApiErrorResponse
    {
        public string Message { get; set; }
        public string Title { get; set; }
    }
}
