using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Verificação 2FA", Exported = false)]
public class TwoFactorActivity : Activity
{
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        SetContentView(Resource.Layout.activity_two_factor);

        FindViewById<Button>(Resource.Id.backButton)!.Click += (_, _) => Finish();
    }
}
