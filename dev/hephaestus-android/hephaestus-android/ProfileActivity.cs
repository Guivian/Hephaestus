using Android.Content;
using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Perfil", Exported = false)]
public class ProfileActivity : Activity
{
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        SetContentView(Resource.Layout.activity_profile);

        FindViewById<TextView>(Resource.Id.profileInitials)!.Text = SessionStore.Initials(this);
        FindViewById<TextView>(Resource.Id.profileName)!.Text = SessionStore.Get(this, "name", "");
        FindViewById<TextView>(Resource.Id.profileEmail)!.Text = SessionStore.Get(this, "email", "");
        FindViewById<TextView>(Resource.Id.profileRole)!.Text = SessionStore.Get(this, "role", "").ToUpperInvariant();
        FindViewById<TextView>(Resource.Id.profileLocation)!.Text = SessionStore.Get(this, "location", "");

        var toggle = FindViewById<Switch>(Resource.Id.twoFactorSwitch)!;
        var status = FindViewById<TextView>(Resource.Id.twoFactorStatus)!;
        var technician = SessionStore.Get(this, "role", "") == "Técnico";

        toggle.Checked = SessionStore.TwoFactor(this);
        toggle.Enabled = false;

        void RefreshTwoFactorStatus()
        {
            status.Text = toggle.Checked ? technician ? "Ativada · obrigatória para técnicos" : "Ativada" : "Desativada";
        }

        RefreshTwoFactorStatus();

        FindViewById<Button>(Resource.Id.changePasswordButton)!.Click += (_, _) =>
            StartActivity(new Intent(this, typeof(ChangePasswordActivity)));

        FindViewById<Button>(Resource.Id.logoutButton)!.Click += (_, _) =>
        {
            var dialog = new AlertDialog.Builder(this);
            dialog.SetTitle("Terminar sessão");
            dialog.SetMessage("Pretende sair da aplicação?");
            dialog.SetNegativeButton("Cancelar", (_, _) => { });
            dialog.SetPositiveButton("Sair", async (_, _) =>
            {
                var refreshToken = SessionStore.RefreshToken(this);
                if (!string.IsNullOrWhiteSpace(refreshToken))
                    await HephaestusApiClient.LogoutAsync(refreshToken);
                SessionStore.SignOut(this);
                StartActivity(new Intent(this, typeof(MainActivity)));
                FinishAffinity();
            });
            dialog.Show();
        };

        BottomNavigation.Setup(this, NavigationTab.Profile);
    }
}
