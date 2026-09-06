using Android.Views;
using Android.Content;

namespace hephaestus_android;

[Activity(Label = "@string/app_name", MainLauncher = true, Exported = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        if (SessionStore.IsSignedIn(this))
        {
            OpenHome();
            return;
        }
        SetContentView(Resource.Layout.activity_main);

        var email = FindViewById<EditText>(Resource.Id.emailInput)!;
        var password = FindViewById<EditText>(Resource.Id.passwordInput)!;
        var loginButton = FindViewById<LinearLayout>(Resource.Id.loginButton)!;
        var googleButton = FindViewById<LinearLayout>(Resource.Id.googleButton)!;

        loginButton.Click += async (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(email.Text) || string.IsNullOrWhiteSpace(password.Text))
            {
                Toast.MakeText(this, "Introduza o e-mail e a palavra-passe.", ToastLength.Long)!.Show();
                return;
            }

            SetEnabled(loginButton, false);
            var result = await HephaestusApiClient.LoginAsync(email.Text.Trim(), password.Text);
            SetEnabled(loginButton, true);
            if (!result.IsSuccess || result.Data is null)
            {
                Toast.MakeText(this, result.Error ?? "Não foi possível iniciar sessão.", ToastLength.Long)!.Show();
                return;
            }

            if (result.Data.RequiresTwoFactor)
            {
                StartActivity(new Intent(this, typeof(TwoFactorActivity))
                    .PutExtra("challenge_id", result.Data.ChallengeId.ToString()));
                return;
            }

            SessionStore.Save(this, result.Data);
            OpenHome();
        };

        googleButton.Click += async (_, _) =>
        {
            SetEnabled(googleButton, false);
            var googleResult = await GoogleIdentityService.GetIdTokenAsync(this);
            if (!googleResult.IsSuccess)
            {
                SetEnabled(googleButton, true);
                if (!googleResult.Cancelled)
                    Toast.MakeText(this, googleResult.Error, ToastLength.Long)!.Show();
                return;
            }

            var result = await HephaestusApiClient.GoogleLoginAsync(googleResult.IdToken!);
            SetEnabled(googleButton, true);
            if (!result.IsSuccess || result.Data is null)
            {
                Toast.MakeText(this, result.Error ?? "O login Google falhou.", ToastLength.Long)!.Show();
                return;
            }

            SessionStore.Save(this, result.Data);
            OpenHome();
        };
    }

    private void OpenHome()
    {
        StartActivity(new Intent(this, typeof(HomeActivity)));
        Finish();
    }

    private static void SetEnabled(View view, bool enabled)
    {
        view.Enabled = enabled;
        view.Alpha = enabled ? 1f : .55f;
    }
}
