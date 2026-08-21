using Android.Content;
using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Início", Exported = false)]
public class HomeActivity : Activity
{
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);

        if (!SessionStore.IsSignedIn(this))
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
            return;
        }

        SetContentView(Resource.Layout.activity_home);

        var name = SessionStore.Get(this, "name", "");
        FindViewById<TextView>(Resource.Id.welcomeText)!.Text = string.IsNullOrWhiteSpace(name)
            ? "Olá"
            : $"Olá, {name.Split(' ')[0]}";
        FindViewById<TextView>(Resource.Id.headerInitials)!.Text = SessionStore.Initials(this);

        BottomNavigation.Setup(this, NavigationTab.Home);
    }
}
