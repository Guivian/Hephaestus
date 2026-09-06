using Android.OS;
using AndroidX.Core.Content;
using AndroidX.Credentials;
using Xamarin.GoogleAndroid.Libraries.Identity.GoogleId;
using Java.Util.Concurrent;

namespace hephaestus_android;

internal sealed record GoogleIdentityResult(bool IsSuccess, string? IdToken, string? Error, bool Cancelled = false);

internal static class GoogleIdentityService
{
    public static async Task<GoogleIdentityResult> GetIdTokenAsync(Activity activity)
    {
        try
        {
            var serverClientId = activity.GetString(Resource.String.google_web_client_id);
            var googleOption = new GetGoogleIdOption.Builder()
                .SetFilterByAuthorizedAccounts(false)
                .SetServerClientId(serverClientId!)
                .SetAutoSelectEnabled(false)
                .Build();
            var request = new GetCredentialRequest.Builder()
                .AddCredentialOption(googleOption)
                .Build();

            var completion = new TaskCompletionSource<GetCredentialResponse>();
            var callback = new CredentialCallback(completion);
            var manager = ICredentialManager.Create(activity);
            manager.GetCredentialAsync(activity, request, new CancellationSignal(),
                ContextCompat.GetMainExecutor(activity)!, callback);

            var response = await completion.Task;
            if (response.Credential is not CustomCredential custom ||
                custom.Type != GoogleIdTokenCredential.TypeGoogleIdTokenCredential)
                return new(false, null, "O Google não devolveu uma credencial compatível.");

            var credential = GoogleIdTokenCredential.CreateFrom(custom.Data);
            return new(true, credential.IdToken, null);
        }
        catch (Exception exception)
        {
            var cancelled = exception.GetType().Name.Contains("Cancellation", StringComparison.OrdinalIgnoreCase);
#if DEBUG
            var error = cancelled ? null : $"Google: {exception.GetType().Name} — {exception.Message}";
#else
            var error = cancelled ? null : "Não foi possível iniciar sessão com o Google.";
#endif
            Android.Util.Log.Error("Hephaestus.Google", exception.ToString());
            return new(false, null, error, cancelled);
        }
    }

    private sealed class CredentialCallback(TaskCompletionSource<GetCredentialResponse> completion) :
        Java.Lang.Object, ICredentialManagerCallback
    {
        public void OnResult(Java.Lang.Object? result)
        {
            if (result is GetCredentialResponse response) completion.TrySetResult(response);
            else completion.TrySetException(new InvalidOperationException("Resposta Google inválida."));
        }

        public void OnError(Java.Lang.Object? error) =>
            completion.TrySetException(new InvalidOperationException(
                error?.ToString() ?? "O Credential Manager não indicou a causa do erro."));
    }
}
