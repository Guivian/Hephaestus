using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace hephaestus_android;

internal static class ActivityFeedRenderer
{
    public static void Render(Android.App.Activity activity, LinearLayout container, IReadOnlyList<ActivityEntry> entries, string emptyText)
    {
        container.RemoveAllViews();
        if (entries.Count == 0)
        {
            container.AddView(Text(activity, emptyText, 13, "#A7AABD"));
            return;
        }

        foreach (var entry in entries)
        {
            var card = new LinearLayout(activity) { Orientation = Orientation.Vertical };
            card.SetPadding(Dp(activity, 14), Dp(activity, 12), Dp(activity, 14), Dp(activity, 12));
            card.Background = Background(activity);
            card.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = Dp(activity, 8)
            };
            card.AddView(Text(activity, entry.Author, 13, "#FFFFFF", true));
            card.AddView(Text(activity, entry.Text, 14, "#D6D8E1"));
            card.AddView(Text(activity, entry.Timestamp.ToString("dd MMM yyyy · HH:mm"), 11, "#A7AABD"));
            container.AddView(card);
        }
    }

    static TextView Text(Android.App.Activity activity, string value, int size, string color, bool bold = false)
    {
        var text = new TextView(activity) { Text = value, TextSize = size, Typeface = bold ? Typeface.DefaultBold : Typeface.Default };
        text.SetTextColor(Color.ParseColor(color));
        return text;
    }

    static GradientDrawable Background(Android.App.Activity activity)
    {
        var drawable = new GradientDrawable();
        drawable.SetColor(Color.ParseColor("#191C24"));
        drawable.SetStroke(Dp(activity, 1), Color.ParseColor("#343A40"));
        drawable.SetCornerRadius(Dp(activity, 10));
        return drawable;
    }

    static int Dp(Android.App.Activity activity, int value) => (int)(value * activity.Resources!.DisplayMetrics!.Density + .5f);
}
