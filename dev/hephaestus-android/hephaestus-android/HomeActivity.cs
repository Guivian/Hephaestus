using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.Views;
using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Início", Exported = false)]
public class HomeActivity : Activity
{
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        if (!SessionStore.IsSignedIn(this)) { StartActivity(new Intent(this, typeof(MainActivity))); Finish(); return; }

        SetContentView(Resource.Layout.activity_home);
        var role = SessionStore.Get(this, "role", "Utilizador");
        var technician = role.Equals("Técnico", StringComparison.OrdinalIgnoreCase);
        var name = SessionStore.Get(this, "name", "");
        FindViewById<TextView>(Resource.Id.welcomeText)!.Text = string.IsNullOrWhiteSpace(name) ? "Olá" : $"Olá, {name.Split(' ')[0]}";
        FindViewById<TextView>(Resource.Id.homeDate)!.Text = DateTime.Today.ToString("dddd, dd 'de' MMMM");
        FindViewById<TextView>(Resource.Id.headerInitials)!.Text = SessionStore.Initials(this);
        RenderConnectivity();

        FindViewById<View>(Resource.Id.technicianHome)!.Visibility = technician ? ViewStates.Visible : ViewStates.Gone;
        FindViewById<View>(Resource.Id.userHome)!.Visibility = technician ? ViewStates.Gone : ViewStates.Visible;
        if (technician) RenderTechnicianHome(); else RenderUserHome();
        BottomNavigation.Setup(this, NavigationTab.Home);
    }

    void RenderConnectivity()
    {
        var manager = (ConnectivityManager)GetSystemService(ConnectivityService)!;
        var capabilities = manager.GetNetworkCapabilities(manager.ActiveNetwork);
        var online = capabilities?.HasCapability(NetCapability.Internet) == true;
        var label = FindViewById<TextView>(Resource.Id.connectivityStatus)!;
        label.Text = online ? "●  Online" : "●  Sem ligação à rede";
        label.SetTextColor(Color.ParseColor(online ? "#00D25B" : "#FC424A"));
    }

    void RenderTechnicianHome()
    {
        var tasks = TaskRepository.All.OrderBy(task => task.ScheduledStart).ToList();
        var today = tasks.Where(task => task.ScheduledStart.Date == DateTime.Today).ToList();
        var inProgress = tasks.Count(task => task.Status == "In Progress");
        var attention = TaskRepository.AllTickets.Count(ticket => ticket.Priority is "P1" or "P2" || ticket.Status is "Open" or "Pending");
        SetMetric(Resource.Id.metricToday, today.Count, "Hoje", () => Open<AgendaActivity>());
        SetMetric(Resource.Id.metricProgress, inProgress, "Em progresso", () => Open<TasksActivity>());
        SetMetric(Resource.Id.metricAttention, attention, "Atenção", () => Open<TicketsActivity>());

        var next = tasks.FirstOrDefault(task => task.ScheduledEnd >= DateTime.Now && task.Status != "Done") ?? tasks.FirstOrDefault(task => task.Status != "Done");
        var nextContainer = FindViewById<LinearLayout>(Resource.Id.nextTask)!;
        if (next is not null) nextContainer.AddView(TaskCard(next, true));
        else nextContainer.AddView(Label("Não existem intervenções atribuídas.", 13, "#A7AABD"));

        var agenda = FindViewById<LinearLayout>(Resource.Id.todayAgenda)!;
        foreach (var task in today.Take(3)) agenda.AddView(TaskCard(task, false));
        if (today.Count == 0) agenda.AddView(Label("Sem intervenções planeadas para hoje.", 13, "#A7AABD"));

        var tickets = FindViewById<LinearLayout>(Resource.Id.attentionTickets)!;
        foreach (var ticket in TaskRepository.AllTickets.Where(ticket => ticket.Priority is "P1" or "P2" || ticket.Status is "Open" or "Pending").Take(3)) tickets.AddView(TicketCard(ticket));
        if (tickets.ChildCount == 0) tickets.AddView(Label("Nenhum ticket requer atenção.", 13, "#A7AABD"));

        var references = tasks.Select(task => task.ReferenceCode).Concat(TaskRepository.AllTickets.Select(ticket => ticket.ReferenceCode));
        ActivityFeedRenderer.Render(this, FindViewById<LinearLayout>(Resource.Id.homeActivity)!, TaskRepository.RecentActivity(references).Take(2).ToList(), "Sem atividade recente.");
        FindViewById<Button>(Resource.Id.viewAllTasks)!.Click += (_, _) => Open<TasksActivity>();
        FindViewById<Button>(Resource.Id.viewAllTickets)!.Click += (_, _) => Open<TicketsActivity>();
    }

    void RenderUserHome()
    {
        FindViewById<Button>(Resource.Id.homeOpenTicket)!.Click += (_, _) => Open<QuickTicketActivity>();
        FindViewById<Button>(Resource.Id.homeViewTickets)!.Click += (_, _) => Open<TicketsActivity>();
        var tickets = FindViewById<LinearLayout>(Resource.Id.userActiveTickets)!;
        foreach (var ticket in TaskRepository.AllTickets.Take(3)) tickets.AddView(TicketCard(ticket));
        if (TaskRepository.AllTickets.Count == 0)
            tickets.AddView(Label("Ainda não existem pedidos ativos.", 13, "#A7AABD"));
        var references = TaskRepository.AllTickets.Select(ticket => ticket.ReferenceCode);
        ActivityFeedRenderer.Render(this, FindViewById<LinearLayout>(Resource.Id.userRecentActivity)!, TaskRepository.RecentActivity(references).Take(2).ToList(), "Sem atividade recente.");
    }

    void SetMetric(int id, int value, string title, Action action)
    {
        var metric = FindViewById<LinearLayout>(id)!;
        ((TextView)metric.GetChildAt(0)!).Text = value.ToString();
        ((TextView)metric.GetChildAt(1)!).Text = title;
        metric.Click += (_, _) => action();
    }

    View TaskCard(TechnicianTask task, bool detailed)
    {
        var card = Card();
        card.AddView(Label($"{task.ReferenceCode}  ·  {task.Status}", 13, StatusColor(task.Status), true));
        card.AddView(Label(task.Title, detailed ? 18 : 15, "#FFFFFF", true));
        card.AddView(Label($"{task.ScheduledStart:HH:mm}–{task.ScheduledEnd:HH:mm}  ·  {task.Location}", 13, "#A7AABD"));
        if (detailed) card.AddView(Label($"{task.TicketReference}  ·  {task.Priority}  ·  {task.Equipment}  ›", 13, "#D6D8E1"));
        card.Click += (_, _) => StartActivity(new Intent(this, typeof(TaskDetailActivity)).PutExtra("task_id", task.Id));
        return card;
    }

    View TicketCard(SupportTicket ticket)
    {
        var card = Card();
        card.AddView(Label($"{ticket.ReferenceCode}  ·  {ticket.Priority}  ·  {ticket.Status}", 13, ticket.Priority is "P1" or "P2" ? "#FFAB00" : "#0090E7", true));
        card.AddView(Label(ticket.Title, 15, "#FFFFFF", true));
        card.Click += (_, _) => StartActivity(new Intent(this, typeof(TicketDetailActivity)).PutExtra("ticket_id", ticket.Id));
        return card;
    }

    LinearLayout Card()
    {
        var card = new LinearLayout(this) { Orientation = Orientation.Vertical, Clickable = true, Focusable = true };
        card.SetPadding(Dp(14), Dp(12), Dp(14), Dp(12));
        card.Background = Background();
        card.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) { BottomMargin = Dp(9) };
        return card;
    }

    TextView Label(string value, int size, string color, bool bold = false) { var text = new TextView(this) { Text = value, TextSize = size, Typeface = bold ? Typeface.DefaultBold : Typeface.Default }; text.SetTextColor(Color.ParseColor(color)); return text; }
    GradientDrawable Background() { var drawable = new GradientDrawable(); drawable.SetColor(Color.ParseColor("#191C24")); drawable.SetStroke(Dp(1), Color.ParseColor("#343A40")); drawable.SetCornerRadius(Dp(10)); return drawable; }
    static string StatusColor(string status) => status switch { "Open" => "#0090E7", "In Progress" => "#FFAB00", "Done" => "#00D25B", _ => "#A7AABD" };
    void Open<TActivity>() where TActivity : Activity => StartActivity(new Intent(this, typeof(TActivity)));
    int Dp(int value) => (int)(value * Resources!.DisplayMetrics!.Density + .5f);
}
