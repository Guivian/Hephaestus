using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Alterar palavra-passe", Exported = false)]
public class ChangePasswordActivity : Activity
{
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        SetContentView(Resource.Layout.activity_change_password);

        FindViewById<Button>(Resource.Id.cancelButton)!.Click += (_, _) => Finish();
    }
}
