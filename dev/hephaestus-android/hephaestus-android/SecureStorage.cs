using Android.Security.Keystore;
using Android.Content;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;

namespace hephaestus_android;

internal static class SecureStorage
{
    private const string Alias = "hephaestus.session.key";
    private const string StoreName = "hephaestus_secure_session";

    public static void Set(Context context, string name, string? value)
    {
        if (string.IsNullOrEmpty(value)) { Remove(context, name); return; }
        var cipher = Cipher.GetInstance("AES/GCM/NoPadding")!;
        cipher.Init(CipherMode.EncryptMode, GetOrCreateKey());
        var encrypted = cipher.DoFinal(System.Text.Encoding.UTF8.GetBytes(value))!;
        var packed = Convert.ToBase64String(cipher.GetIV()!) + "." + Convert.ToBase64String(encrypted);
        Prefs(context).Edit()!.PutString(name, packed)!.Commit();
    }

    public static string? Get(Context context, string name)
    {
        var packed = Prefs(context).GetString(name, null);
        if (string.IsNullOrWhiteSpace(packed)) return null;
        try
        {
            var parts = packed.Split('.');
            if (parts.Length != 2) return null;
            var cipher = Cipher.GetInstance("AES/GCM/NoPadding")!;
            cipher.Init(CipherMode.DecryptMode, GetOrCreateKey(),
                new GCMParameterSpec(128, Convert.FromBase64String(parts[0])));
            return System.Text.Encoding.UTF8.GetString(cipher.DoFinal(Convert.FromBase64String(parts[1]))!);
        }
        catch (Exception) { return null; }
    }

    public static void Clear(Context context) => Prefs(context).Edit()!.Clear()!.Commit();
    private static void Remove(Context context, string name) => Prefs(context).Edit()!.Remove(name)!.Commit();
    private static ISharedPreferences Prefs(Context context) =>
        context.GetSharedPreferences(StoreName, FileCreationMode.Private)!;

    private static IKey GetOrCreateKey()
    {
        var store = KeyStore.GetInstance("AndroidKeyStore")!;
        store.Load(null);
        if (store.ContainsAlias(Alias)) return store.GetKey(Alias, null)!;

        var generator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, "AndroidKeyStore")!;
        var specification = new KeyGenParameterSpec.Builder(Alias,
                KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
            .SetBlockModes(KeyProperties.BlockModeGcm)
            .SetEncryptionPaddings(KeyProperties.EncryptionPaddingNone)
            .Build();
        generator.Init(specification);
        return generator.GenerateKey()!;
    }
}
