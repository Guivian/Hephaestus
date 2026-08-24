using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Detalhe do ticket", Exported = false)]
public class TicketDetailActivity : Activity
{
    SupportTicket ticket = null!;

    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        if (!SessionStore.IsSignedIn(this)) { StartActivity(new Intent(this, typeof(MainActivity))); Finish(); return; }
        ticket = TaskRepository.FindTicket(Intent?.GetIntExtra("ticket_id", -1) ?? -1)!;
        if (ticket is null) { Finish(); return; }
        SetContentView(Resource.Layout.activity_ticket_detail);
        FindViewById<Button>(Resource.Id.ticketDetailBack)!.Click += (_, _) => Finish();
        FindViewById<TextView>(Resource.Id.ticketDetailReference)!.Text = ticket.ReferenceCode;
        FindViewById<TextView>(Resource.Id.ticketDetailStatus)!.Text = ticket.Status;
        FindViewById<TextView>(Resource.Id.ticketDetailTitle)!.Text = ticket.Title;
        FindViewById<TextView>(Resource.Id.ticketDetailDescription)!.Text = ticket.Description;
        FindViewById<TextView>(Resource.Id.ticketDetailMetadata)!.Text = $"{ticket.Type}  ·  {ticket.Priority}\n{ticket.Location}\n{ticket.Equipment}\nAberto por {ticket.CreatedBy}";
        var tasks = TaskRepository.ForTicket(ticket.ReferenceCode);
        FindViewById<TextView>(Resource.Id.interventionCount)!.Text = $"{tasks.Count} INTERVENÇÕES";
        var container = FindViewById<LinearLayout>(Resource.Id.ticketInterventions)!;
        foreach (var task in tasks) container.AddView(CreateTaskCard(task));
        FindViewById<Button>(Resource.Id.ticketCommentsViewAll)!.Click += (_, _) => OpenActivityFeed(true);
        FindViewById<Button>(Resource.Id.ticketHistoryViewAll)!.Click += (_, _) => OpenActivityFeed(false);
        ActivityFeedRenderer.Render(this, FindViewById<LinearLayout>(Resource.Id.ticketComments)!, TaskRepository.CommentsFor(ticket.ReferenceCode).Take(2).ToList(), "Ainda não existem comentários.");
        ActivityFeedRenderer.Render(this, FindViewById<LinearLayout>(Resource.Id.ticketHistory)!, TaskRepository.HistoryFor(ticket.ReferenceCode).Take(2).ToList(), "Ainda não existe histórico.");
        BottomNavigation.Setup(this, NavigationTab.Tickets);
    }

    View CreateTaskCard(TechnicianTask task)
    {
        var card = new LinearLayout(this) { Orientation = Orientation.Vertical, Clickable = true, Focusable = true };
        card.Background = Background();
        card.SetPadding(Dp(16), Dp(14), Dp(16), Dp(14));
        card.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) { BottomMargin = Dp(10) };
        var header = new LinearLayout(this) { Orientation = Orientation.Horizontal };
        var code = Text(task.ReferenceCode, 14, "#0090E7", true); code.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1); header.AddView(code);
        header.AddView(Text(task.Status, 12, task.Status == "Done" ? "#00D25B" : task.Status == "In Progress" ? "#FFAB00" : "#0090E7", true));
        card.AddView(header);
        var title = Text(task.Title, 16, "#FFFFFF", true); title.SetPadding(0, Dp(8), 0, Dp(5)); card.AddView(title);
        card.AddView(Text($"{task.ScheduledStart:dd MMM · HH:mm}–{task.ScheduledEnd:HH:mm}  ·  {task.Technician}  ›", 13, "#A7AABD"));
        card.Click += (_, _) => StartActivity(new Intent(this, typeof(TaskDetailActivity)).PutExtra("task_id", task.Id));
        return card;
    }

    TextView Text(string value, int size, string color, bool bold = false) { var text = new TextView(this) { Text = value, TextSize = size, Typeface = bold ? Typeface.DefaultBold : Typeface.Default }; text.SetTextColor(Color.ParseColor(color)); return text; }
    GradientDrawable Background() { var drawable = new GradientDrawable(); drawable.SetColor(Color.ParseColor("#191C24")); drawable.SetStroke(Dp(1), Color.ParseColor("#343A40")); drawable.SetCornerRadius(Dp(10)); return drawable; }
    int Dp(int value) => (int)(value * Resources!.DisplayMetrics!.Density + .5f);

    void OpenActivityFeed(bool comments) => StartActivity(new Intent(this, typeof(ActivityFeedActivity))
        .PutExtra("reference", ticket.ReferenceCode)
        .PutExtra("comments", comments));
}
