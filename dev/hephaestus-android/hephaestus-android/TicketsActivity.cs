using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Tickets", Exported = false)]
public class TicketsActivity : Activity
{
    LinearLayout list = null!;

    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        if (!SessionStore.IsSignedIn(this)) { StartActivity(new Intent(this, typeof(MainActivity))); Finish(); return; }
        SetContentView(Resource.Layout.activity_tickets);
        list = FindViewById<LinearLayout>(Resource.Id.ticketList)!;
        FindViewById<TextView>(Resource.Id.headerInitials)!.Text = SessionStore.Initials(this);
        FindViewById<Button>(Resource.Id.openTicketButton)!.Click += (_, _) => StartActivity(new Intent(this, typeof(QuickTicketActivity)));
        BottomNavigation.Setup(this, NavigationTab.Tickets);
        Render();
    }

    void Render()
    {
        list.RemoveAllViews();
        foreach (var ticket in TaskRepository.AllTickets) list.AddView(CreateCard(ticket));
        if (TaskRepository.AllTickets.Count == 0)
            list.AddView(Text("Ainda não existem tickets para apresentar.", 14, "#A7AABD"));
    }

    View CreateCard(SupportTicket ticket)
    {
        var card = new LinearLayout(this) { Orientation = Orientation.Vertical, Clickable = true, Focusable = true };
        card.Background = Background("#191C24", ticket.ReferenceCode.StartsWith("SUP") ? "#0090E7" : "#00D25B");
        card.SetPadding(Dp(16), Dp(15), Dp(16), Dp(15));
        card.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) { BottomMargin = Dp(12) };
        var header = new LinearLayout(this) { Orientation = Orientation.Horizontal };
        var reference = Text(ticket.ReferenceCode, 14, ticket.ReferenceCode.StartsWith("SUP") ? "#0090E7" : "#00D25B", true);
        reference.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1);
        header.AddView(reference);
        header.AddView(Text(ticket.Status, 12, "#FFAB00", true));
        card.AddView(header);
        var title = Text(ticket.Title, 17, "#FFFFFF", true); title.SetPadding(0, Dp(10), 0, Dp(7)); card.AddView(title);
        var count = TaskRepository.ForTicket(ticket.ReferenceCode).Count;
        card.AddView(Text($"{ticket.Type}  ·  {ticket.Priority}  ·  {count} intervenções", 13, "#D6D8E1"));
        card.AddView(Text($"⌖  {ticket.Location}", 13, "#A7AABD"));
        card.Click += (_, _) => StartActivity(new Intent(this, typeof(TicketDetailActivity)).PutExtra("ticket_id", ticket.Id));
        return card;
    }

    TextView Text(string value, int size, string color, bool bold = false) { var text = new TextView(this) { Text = value, TextSize = size, Typeface = bold ? Typeface.DefaultBold : Typeface.Default }; text.SetTextColor(Color.ParseColor(color)); return text; }
    GradientDrawable Background(string fill, string stroke) { var drawable = new GradientDrawable(); drawable.SetColor(Color.ParseColor(fill)); drawable.SetStroke(Dp(1), Color.ParseColor(stroke)); drawable.SetCornerRadius(Dp(12)); return drawable; }
    int Dp(int value) => (int)(value * Resources!.DisplayMetrics!.Density + .5f);
}
