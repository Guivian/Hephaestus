namespace hephaestus_android;

[Activity(Label = "@string/app_name", MainLauncher = true, Exported = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (SessionStore.IsSignedIn(this))
        {
            StartActivity(new Android.Content.Intent(this, typeof(HomeActivity)));
            Finish();
            return;
        }

        SetContentView(Resource.Layout.activity_main);

    }
}
