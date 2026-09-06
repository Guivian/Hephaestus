using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace hephaestus_web
{
    public sealed class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
    }

    public static class ApiClient
    {
        private static readonly HttpClient Client = CreateClient();
        private static readonly JavaScriptSerializer Json = new JavaScriptSerializer();

        public static Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password)
        {
            return SendAsync<LoginResponse>(HttpMethod.Post, "api/auth/login",
                new { email = email, password = password });
        }

        public static Task<ApiResponse<LoginResponse>> StartLoginAsync(string email, string password)
        {
            return SendAsync<LoginResponse>(HttpMethod.Post, "api/auth/login",
                new { email = email, password = password });
        }

        public static Task<ApiResponse<LoginResponse>> VerifyTwoFactorAsync(Guid challengeId, string code)
        {
            return SendAsync<LoginResponse>(HttpMethod.Post, "api/auth/2fa/verify",
                new { challengeId = challengeId, code = code });
        }

        public static Task<ApiResponse<LoginResponse>> ResendTwoFactorAsync(Guid challengeId)
        {
            return SendAsync<LoginResponse>(HttpMethod.Post, "api/auth/2fa/resend",
                new { challengeId = challengeId });
        }

        public static Task<ApiResponse<LoginResponse>> ExchangeGoogleCodeAsync(string code)
        {
            return SendAsync<LoginResponse>(HttpMethod.Post, "api/auth/google/web/exchange",
                new { code = code });
        }

        public static Task<ApiResponse<LoginResponse>> RefreshAsync(string refreshToken)
        {
            return SendAsync<LoginResponse>(HttpMethod.Post, "api/auth/refresh",
                new { refreshToken = refreshToken });
        }

        public static Task<ApiResponse<object>> LogoutAsync(string refreshToken)
        {
            return SendAsync<object>(HttpMethod.Post, "api/auth/logout",
                new { refreshToken = refreshToken });
        }

        public static async Task<ApiResponse<T>> SendAuthorizedAsync<T>(
            HttpMethod method, string relativeUrl, object body = null)
        {
            if (!WebSession.EnsureFreshAccessToken())
                return new ApiResponse<T> { Error = "A sessão terminou. Inicie sessão novamente." };

            return await SendAsync<T>(method, relativeUrl, body, WebSession.AccessToken)
                .ConfigureAwait(false);
        }

        private static async Task<ApiResponse<T>> SendAsync<T>(
            HttpMethod method, string relativeUrl, object body, string bearerToken = null)
        {
            try
            {
                using (var request = new HttpRequestMessage(method, relativeUrl))
                {
                    if (body != null)
                    {
                        request.Content = new StringContent(
                            Json.Serialize(body), Encoding.UTF8, "application/json");
                    }

                    if (!string.IsNullOrWhiteSpace(bearerToken))
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    using (var response = await Client.SendAsync(request).ConfigureAwait(false))
                    {
                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            return new ApiResponse<T>
                            {
                                IsSuccess = true,
                                StatusCode = response.StatusCode,
                                Data = string.IsNullOrWhiteSpace(content) ? default(T) : Json.Deserialize<T>(content)
                            };
                        }

                        return new ApiResponse<T>
                        {
                            StatusCode = response.StatusCode,
                            Error = ReadError(content, response.ReasonPhrase)
                        };
                    }
                }
            }
            catch (HttpRequestException)
            {
                return new ApiResponse<T> { Error = "Não foi possível contactar a API local." };
            }
            catch (TaskCanceledException)
            {
                return new ApiResponse<T> { Error = "A API demorou demasiado tempo a responder." };
            }
            catch (Exception exception)
            {
                return new ApiResponse<T>
                {
                    Error = "Não foi possível concluir o pedido à API: " + exception.Message
                };
            }
        }

        private static string ReadError(string content, string fallback)
        {
            try
            {
                var error = Json.Deserialize<ApiErrorResponse>(content);
                if (!string.IsNullOrWhiteSpace(error.Message)) return error.Message;
                if (!string.IsNullOrWhiteSpace(error.Title)) return error.Title;
            }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }

            return string.IsNullOrWhiteSpace(fallback) ? "O pedido à API falhou." : fallback;
        }

        private static HttpClient CreateClient()
        {
            var baseUrl = ConfigurationManager.AppSettings["HephaestusApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ConfigurationErrorsException("HephaestusApiBaseUrl não está configurado.");

            return new HttpClient
            {
                BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
                Timeout = TimeSpan.FromSeconds(15)
            };
        }
    }
}
