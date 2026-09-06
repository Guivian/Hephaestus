using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Android.Content;

namespace hephaestus_android;

internal static class HephaestusApiClient
{
#if DEBUG
    private const string BaseUrl = "http://10.0.2.2:5022/";
#else
    private const string BaseUrl = "https://10.0.2.2:7225/";
#endif
    private static readonly HttpClient Client = new() { BaseAddress = new Uri(BaseUrl), Timeout = TimeSpan.FromSeconds(20) };
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    public static Task<ApiResult<AuthenticationResponse>> LoginAsync(string email, string password) =>
        SendAsync<AuthenticationResponse>(HttpMethod.Post, "api/auth/login", new LoginRequest(email, password));

    public static Task<ApiResult<AuthenticationResponse>> VerifyTwoFactorAsync(Guid challengeId, string code) =>
        SendAsync<AuthenticationResponse>(HttpMethod.Post, "api/auth/2fa/verify", new TwoFactorRequest(challengeId, code));

    public static Task<ApiResult<AuthenticationResponse>> ResendTwoFactorAsync(Guid challengeId) =>
        SendAsync<AuthenticationResponse>(HttpMethod.Post, "api/auth/2fa/resend", new ChallengeRequest(challengeId));

    public static Task<ApiResult<AuthenticationResponse>> GoogleLoginAsync(string idToken) =>
        SendAsync<AuthenticationResponse>(HttpMethod.Post, "api/auth/google/mobile", new GoogleLoginRequest(idToken));

    public static Task<ApiResult<object>> LogoutAsync(string refreshToken) =>
        SendAsync<object>(HttpMethod.Post, "api/auth/logout", new RefreshTokenRequest(refreshToken));

    public static async Task<ApiResult<T>> SendAuthorizedAsync<T>(
        Context context, HttpMethod method, string path, object? body = null)
    {
        var first = await SendAsync<T>(method, path, body, SessionStore.AccessToken(context));
        if (first.StatusCode != (int)HttpStatusCode.Unauthorized) return first;

        if (!await RefreshSessionAsync(context)) return first;
        return await SendAsync<T>(method, path, body, SessionStore.AccessToken(context));
    }

    private static async Task<bool> RefreshSessionAsync(Context context)
    {
        await RefreshLock.WaitAsync();
        try
        {
            var refreshToken = SessionStore.RefreshToken(context);
            if (string.IsNullOrWhiteSpace(refreshToken)) return false;

            var result = await SendAsync<AuthenticationResponse>(
                HttpMethod.Post, "api/auth/refresh", new RefreshTokenRequest(refreshToken));
            if (!result.IsSuccess || result.Data is null)
            {
                SessionStore.SignOut(context);
                return false;
            }

            SessionStore.Save(context, result.Data);
            return true;
        }
        finally
        {
            RefreshLock.Release();
        }
    }

    private static async Task<ApiResult<T>> SendAsync<T>(
        HttpMethod method, string path, object? body, string? accessToken = null)
    {
        try
        {
            using var request = new HttpRequestMessage(method, path);
            if (body is not null)
            {
                var bodyType = ApiJsonContext.Default.GetTypeInfo(body.GetType())
                    ?? throw new InvalidOperationException("O tipo do pedido JSON não está registado.");
                request.Content = JsonContent.Create(body, bodyType);
            }
            if (!string.IsNullOrWhiteSpace(accessToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await Client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return new ApiResult<T>
                {
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                    Data = string.IsNullOrWhiteSpace(content) ? default : Deserialize<T>(content)
                };
            }

            return new ApiResult<T>
            {
                StatusCode = (int)response.StatusCode,
                Error = ReadError(content) ?? $"O pedido falhou ({(int)response.StatusCode})."
            };
        }
        catch (HttpRequestException)
        {
            return new ApiResult<T> { Error = $"Não foi possível contactar a API em {BaseUrl}" };
        }
        catch (TaskCanceledException)
        {
            return new ApiResult<T> { Error = "A API demorou demasiado tempo a responder." };
        }
    }

    private static string? ReadError(string content)
    {
        try
        {
            var error = JsonSerializer.Deserialize(content, ApiJsonContext.Default.ApiErrorResponse);
            return error?.Message ?? error?.Title;
        }
        catch (JsonException) { return null; }
    }

    private static T? Deserialize<T>(string content)
    {
        var typeInfo = ApiJsonContext.Default.GetTypeInfo(typeof(T))
            ?? throw new InvalidOperationException("O tipo da resposta JSON não está registado.");
        return (T?)JsonSerializer.Deserialize(content, typeInfo);
    }
}
