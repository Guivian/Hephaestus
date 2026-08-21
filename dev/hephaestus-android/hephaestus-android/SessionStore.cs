using Android.Content;

namespace hephaestus_android;

internal static class SessionStore
{
    const string Store = "hephaestus_session";
    static ISharedPreferences Prefs(Context context) => context.GetSharedPreferences(Store, FileCreationMode.Private)!;

    public static bool IsSignedIn(Context c) => Prefs(c).GetBoolean("signed_in", false);
    public static string Get(Context c, string key, string fallback) => Prefs(c).GetString(key, fallback) ?? fallback;
    public static bool TwoFactor(Context c) => Prefs(c).GetBoolean("two_factor", false);

    public static void SignOut(Context c) => Prefs(c).Edit()!.PutBoolean("signed_in", false)!.Remove("name")!.Remove("email")!.Remove("role")!.Apply();

    public static string Initials(Context c) => string.Concat(
        Get(c, "name", "")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(part => char.ToUpperInvariant(part[0])));
}
