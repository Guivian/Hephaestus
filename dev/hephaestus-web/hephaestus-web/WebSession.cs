using System;
using System.Web;

namespace hephaestus_web
{
    public static class WebSession
    {
        private static HttpSessionStateBase Session =>
            new HttpSessionStateWrapper(HttpContext.Current.Session);

        public static bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken);
        public static string AccessToken => Session["AccessToken"] as string;
        public static string RefreshToken => Session["RefreshToken"] as string;
        public static string Role => Session["Role"] as string;

        public static void Store(LoginResponse response)
        {
            Session["AccessToken"] = response.AccessToken;
            Session["RefreshToken"] = response.RefreshToken;
            Session["ExpiresAt"] = response.ExpiresAt;
            Session["UserId"] = response.UserId;
            Session["Name"] = response.Name;
            Session["Email"] = response.Email;
            Session["Role"] = response.Role;
        }

        public static bool EnsureFreshAccessToken()
        {
            if (!IsAuthenticated || string.IsNullOrWhiteSpace(RefreshToken)) return false;

            var expiresAt = Session["ExpiresAt"] is DateTime value ? value : DateTime.MinValue;
            if (expiresAt > DateTime.UtcNow.AddMinutes(1)) return true;

            var result = ApiClient.RefreshAsync(RefreshToken).GetAwaiter().GetResult();
            if (!result.IsSuccess || result.Data == null)
            {
                Clear();
                return false;
            }

            Store(result.Data);
            return true;
        }

        public static void Clear()
        {
            Session.Clear();
            Session.Abandon();
        }
    }
}
