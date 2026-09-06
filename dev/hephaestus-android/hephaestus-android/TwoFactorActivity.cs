using Android.Widget;
using Android.Content;
using Android.Views;

namespace hephaestus_android;

[Activity(Label = "Verificação 2FA", Exported = false)]
public class TwoFactorActivity : Activity
{
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        SetContentView(Resource.Layout.activity_two_factor);

        var challengeText = Intent?.GetStringExtra("challenge_id");
        if (!Guid.TryParse(challengeText, out var challengeId))
        {
            Toast.MakeText(this, "O desafio 2FA não é válido.", ToastLength.Long)!.Show();
            Finish();
            return;
        }

        var code = FindViewById<EditText>(Resource.Id.codeInput)!;
        var error = FindViewById<TextView>(Resource.Id.codeError)!;
        var confirm = FindViewById<Button>(Resource.Id.confirmButton)!;
        var resend = FindViewById<Button>(Resource.Id.resendButton)!;

        confirm.Click += async (_, _) =>
        {
            if (code.Text?.Length != 6 || !int.TryParse(code.Text, out _))
            {
                ShowError(error, "Introduza os seis números do código.");
                return;
            }

            confirm.Enabled = false;
            var result = await HephaestusApiClient.VerifyTwoFactorAsync(challengeId, code.Text);
            confirm.Enabled = true;
            if (!result.IsSuccess || result.Data is null)
            {
                ShowError(error, result.Error ?? "O código não é válido.");
                return;
            }

            SessionStore.Save(this, result.Data);
            StartActivity(new Intent(this, typeof(HomeActivity)));
            FinishAffinity();
        };

        resend.Click += async (_, _) =>
        {
            resend.Enabled = false;
            var result = await HephaestusApiClient.ResendTwoFactorAsync(challengeId);
            resend.Enabled = true;
            if (!result.IsSuccess || result.Data is null)
            {
                ShowError(error, result.Error ?? "Não foi possível reenviar o código.");
                return;
            }

            challengeId = result.Data.ChallengeId;
            error.Visibility = ViewStates.Visible;
            error.SetTextColor(Android.Graphics.Color.ParseColor("#00D25B"));
            error.Text = "Foi enviado um novo código.";
        };

        FindViewById<Button>(Resource.Id.backButton)!.Click += (_, _) => Finish();
    }

    private static void ShowError(TextView view, string message)
    {
        view.SetTextColor(Android.Graphics.Color.ParseColor("#FC424A"));
        view.Text = message;
        view.Visibility = ViewStates.Visible;
    }
}
