using Android.Content;

namespace hephaestus_android;

internal static class SessionStore
{
    const string Store = "hephaestus_session";
    static ISharedPreferences Prefs(Context context) => context.GetSharedPreferences(Store, FileCreationMode.Private)!;

    public static bool IsSignedIn(Context c) => !string.IsNullOrWhiteSpace(AccessToken(c)) &&
        !string.IsNullOrWhiteSpace(RefreshToken(c));
    public static string Get(Context c, string key, string fallback) => SecureStorage.Get(c, key) ?? fallback;
    public static string? AccessToken(Context c) => SecureStorage.Get(c, "access_token");
    public static string? RefreshToken(Context c) => SecureStorage.Get(c, "refresh_token");
    public static bool TwoFactor(Context c) => Get(c, "two_factor", "false") == "true";

    public static void Save(Context context, AuthenticationResponse response)
    {
        SecureStorage.Set(context, "access_token", response.AccessToken);
        SecureStorage.Set(context, "refresh_token", response.RefreshToken);
        SecureStorage.Set(context, "expires_at", response.ExpiresAt.ToUniversalTime().ToString("O"));
        SecureStorage.Set(context, "user_id", response.UserId.ToString());
        SecureStorage.Set(context, "name", response.Name);
        SecureStorage.Set(context, "email", response.Email);
        SecureStorage.Set(context, "role", response.Role);
        SecureStorage.Set(context, "two_factor",
            string.Equals(response.Role, "Técnico", StringComparison.OrdinalIgnoreCase) ? "true" : "false");
    }

    public static void SignOut(Context c)
    {
        SecureStorage.Clear(c);
        Prefs(c).Edit()!.Clear()!.Apply();
    }

    public static string Initials(Context c) => string.Concat(
        Get(c, "name", "")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(part => char.ToUpperInvariant(part[0])));
}
