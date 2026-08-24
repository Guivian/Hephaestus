using Android.Content;
using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Atividade", Exported = false)]
public class ActivityFeedActivity : Activity
{
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        if (!SessionStore.IsSignedIn(this)) { StartActivity(new Intent(this, typeof(MainActivity))); Finish(); return; }

        var reference = Intent?.GetStringExtra("reference") ?? "";
        var comments = Intent?.GetBooleanExtra("comments", true) ?? true;
        SetContentView(Resource.Layout.activity_feed);
        FindViewById<Button>(Resource.Id.activityFeedBack)!.Click += (_, _) => Finish();
        FindViewById<TextView>(Resource.Id.activityFeedTitle)!.Text = comments ? "Comentários" : "Histórico de alterações";
        FindViewById<TextView>(Resource.Id.activityFeedReference)!.Text = reference;

        var entries = comments ? TaskRepository.CommentsFor(reference) : TaskRepository.HistoryFor(reference);
        ActivityFeedRenderer.Render(this, FindViewById<LinearLayout>(Resource.Id.activityFeedList)!, entries,
            comments ? "Ainda não existem comentários." : "Ainda não existe histórico.");
        BottomNavigation.Setup(this, reference.StartsWith("INT", StringComparison.OrdinalIgnoreCase) ? NavigationTab.Tasks : NavigationTab.Tickets);
    }
}
