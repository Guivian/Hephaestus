using Android.Content;
using Android.Graphics;
using Android.Widget;

namespace hephaestus_android;

internal enum NavigationTab
{
    Home,
    Tickets,
    Profile
}

internal static class BottomNavigation
{
    static readonly Color ActiveColor = Color.ParseColor("#0090E7");
    static readonly Color InactiveColor = Color.ParseColor("#A7AABD");

    public static void Setup(Activity activity, NavigationTab activeTab)
    {
        ConfigureTab(activity, Resource.Id.homeTabIcon, Resource.Id.homeTabLabel, activeTab == NavigationTab.Home);
        ConfigureTab(activity, Resource.Id.ticketsTabIcon, Resource.Id.ticketsTabLabel, activeTab == NavigationTab.Tickets);
        ConfigureTab(activity, Resource.Id.profileTabIcon, Resource.Id.profileTabLabel, activeTab == NavigationTab.Profile);

        activity.FindViewById<Android.Views.View>(Resource.Id.homeTab)!.Click += (_, _) =>
            Navigate<HomeActivity>(activity, activeTab != NavigationTab.Home);

        activity.FindViewById<Android.Views.View>(Resource.Id.profileTab)!.Click += (_, _) =>
            Navigate<ProfileActivity>(activity, activeTab != NavigationTab.Profile);
    }

    static void ConfigureTab(Activity activity, int iconId, int labelId, bool active)
    {
        var color = active ? ActiveColor : InactiveColor;
        var icon = activity.FindViewById<ImageView>(iconId)!;
        var label = activity.FindViewById<TextView>(labelId)!;

        icon.SetColorFilter(color);
        label.SetTextColor(color);
        label.SetTypeface(null, active ? TypefaceStyle.Bold : TypefaceStyle.Normal);
    }

    static void Navigate<TActivity>(Activity activity, bool shouldNavigate)
        where TActivity : Activity
    {
        if (!shouldNavigate)
        {
            return;
        }

        activity.StartActivity(new Intent(activity, typeof(TActivity)));
        activity.Finish();
    }
}
