using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Tarefas", Exported = false)]
public class TasksActivity : Activity
{
    LinearLayout taskList = null!;
    EditText search = null!;
    TextView resultCount = null!;
    string selectedStatus = "Todas";

    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);

        if (!SessionStore.IsSignedIn(this))
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();

            return;
        }

        SetContentView(Resource.Layout.activity_tasks);

        taskList = FindViewById<LinearLayout>(Resource.Id.taskList)!;
        search = FindViewById<EditText>(Resource.Id.taskSearch)!;
        resultCount = FindViewById<TextView>(Resource.Id.taskResultCount)!;

        FindViewById<TextView>(Resource.Id.headerInitials)!.Text = SessionStore.Initials(this);

        search.TextChanged += (_, _) => RenderTasks();

        ConfigureFilter(Resource.Id.filterAll, "Todas");
        ConfigureFilter(Resource.Id.filterOpen, "Open");
        ConfigureFilter(Resource.Id.filterProgress, "In Progress");
        ConfigureFilter(Resource.Id.filterDone, "Done");

        BottomNavigation.Setup(this, NavigationTab.Tasks);
    }

    protected override void OnResume()
    {
        base.OnResume();
        RenderTasks();
    }

    void ConfigureFilter(int id, string status)
    {
        FindViewById<Button>(id)!.Click += (_, _) =>
        {
            selectedStatus = status;

            search.ClearFocus();

            ((InputMethodManager)GetSystemService(InputMethodService)!).HideSoftInputFromWindow(search.WindowToken, HideSoftInputFlags.None);

            RenderTasks();
        };
    }

    void RenderTasks()
    {
        taskList.RemoveAllViews();
        var term = search.Text?.Trim() ?? "";
        var tasks = TaskRepository.All
            .Where(task => selectedStatus == "Todas" || task.Status == selectedStatus)
            .Where(task => term.Length == 0 || task.ReferenceCode.Contains(term, StringComparison.OrdinalIgnoreCase) || task.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
            .OrderBy(task => task.ScheduledStart)
            .ToList();

        resultCount.Text = tasks.Count == 1 ? "1 tarefa" : $"{tasks.Count} tarefas";
        UpdateFilterAppearance();
        foreach (var task in tasks) taskList.AddView(CreateTaskCard(task));
        FindViewById<View>(Resource.Id.emptyTasks)!.Visibility = tasks.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
    }

    View CreateTaskCard(TechnicianTask task)
    {
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            Clickable = true,
            Focusable = true,
            Background = RoundedBackground("#191C24", "#343A40", 12)
        };
        card.SetPadding(Dp(16), Dp(15), Dp(16), Dp(15));
        card.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) { BottomMargin = Dp(12) };

        var top = new LinearLayout(this) { Orientation = Orientation.Horizontal };
        top.SetGravity(GravityFlags.CenterVertical);
        var reference = Label(task.ReferenceCode, 14, "#0090E7", true);
        reference.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1);
        top.AddView(reference);
        var status = Label(task.Status, 12, StatusColor(task.Status), true);
        status.SetPadding(Dp(10), Dp(5), Dp(10), Dp(5));
        status.Background = RoundedBackground(StatusSurface(task.Status), StatusColor(task.Status), 20);
        top.AddView(status);
        card.AddView(top);

        var title = Label(task.Title, 17, "#FFFFFF", true);
        title.SetPadding(0, Dp(10), 0, Dp(7));
        card.AddView(title);
        card.AddView(Label($"{task.ScheduledStart:ddd, dd MMM · HH:mm}–{task.ScheduledEnd:HH:mm}", 14, "#D6D8E1"));
        card.AddView(Label($"⌖  {task.Location}     ·     {task.Priority}", 13, "#A7AABD"));
        card.Click += (_, _) => StartActivity(new Intent(this, typeof(TaskDetailActivity)).PutExtra("task_id", task.Id));
        return card;
    }

    void UpdateFilterAppearance()
    {
        foreach (var pair in new[] { (Resource.Id.filterAll, "Todas"), (Resource.Id.filterOpen, "Open"), (Resource.Id.filterProgress, "In Progress"), (Resource.Id.filterDone, "Done") })
        {
            var button = FindViewById<Button>(pair.Item1)!;
            var active = selectedStatus == pair.Item2;
            button.SetTextColor(Color.ParseColor(active ? "#FFFFFF" : "#A7AABD"));
            button.Background = RoundedBackground(active ? "#0090E7" : "#191C24", active ? "#0090E7" : "#343A40", 20);
        }
    }

    TextView Label(string text, int size, string color, bool bold = false)
    {
        var label = new TextView(this) { Text = text, TextSize = size, Typeface = bold ? Typeface.DefaultBold : Typeface.Default };
        label.SetTextColor(Color.ParseColor(color));
        return label;
    }
    GradientDrawable RoundedBackground(string fill, string stroke, int radius) { var d = new GradientDrawable(); d.SetColor(Color.ParseColor(fill)); d.SetStroke(Dp(1), Color.ParseColor(stroke)); d.SetCornerRadius(Dp(radius)); return d; }
    static string StatusColor(string status) => status switch { "Open" => "#0090E7", "In Progress" => "#FFAB00", "Done" => "#00D25B", _ => "#A7AABD" };
    static string StatusSurface(string status) => status switch { "Open" => "#132B3A", "In Progress" => "#332B12", "Done" => "#123021", _ => "#242833" };
    int Dp(int value) => (int)(value * Resources!.DisplayMetrics!.Density + .5f);
}
