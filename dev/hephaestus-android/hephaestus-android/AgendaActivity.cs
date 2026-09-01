using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using System.Globalization;

namespace hephaestus_android;

[Activity(Label = "Agenda diária", Exported = false)]
public class AgendaActivity : Activity
{
    LinearLayout agendaList = null!;
    TextView datePickerLabel = null!;
    TextView agendaSummary = null!;
    DateTime selectedDate = DateTime.Today;

    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        if (!SessionStore.IsSignedIn(this)) { StartActivity(new Intent(this, typeof(MainActivity))); Finish(); return; }

        SetContentView(Resource.Layout.activity_agenda);
        agendaList = FindViewById<LinearLayout>(Resource.Id.agendaList)!;
        datePickerLabel = FindViewById<TextView>(Resource.Id.agendaDatePickerLabel)!;
        agendaSummary = FindViewById<TextView>(Resource.Id.agendaSummary)!;
        FindViewById<TextView>(Resource.Id.headerInitials)!.Text = SessionStore.Initials(this);
        FindViewById<Button>(Resource.Id.viewAllTasks)!.Click += (_, _) => StartActivity(new Intent(this, typeof(TasksActivity)));
        FindViewById<View>(Resource.Id.agendaDatePicker)!.Click += (_, _) => OpenDatePicker();
        FindViewById<View>(Resource.Id.previousDay)!.Click += (_, _) => { selectedDate = selectedDate.AddDays(-1); Render(); };
        FindViewById<View>(Resource.Id.nextDay)!.Click += (_, _) => { selectedDate = selectedDate.AddDays(1); Render(); };
        BottomNavigation.Setup(this, NavigationTab.Agenda);
        Render();
    }

    protected override void OnResume() { base.OnResume(); Render(); }

    void Render()
    {
        agendaList.RemoveAllViews();
        var culture = CultureInfo.GetCultureInfo("pt-PT");
        var weekDay = selectedDate.ToString("ddd", culture).TrimEnd('.');
        datePickerLabel.Text = $"{weekDay}, {selectedDate.ToString("dd MMM", culture).TrimEnd('.')}".ToUpperInvariant();

        var tasks = TaskRepository.All.Where(task => task.ScheduledStart.Date == selectedDate.Date).OrderBy(task => task.ScheduledStart).ToList();
        agendaSummary.Text = tasks.Count == 1 ? "1 intervenção planeada" : $"{tasks.Count} intervenções planeadas";
        if (tasks.Count == 0)
        {
            var empty = Label("Sem intervenções planeadas para este dia.", 14, "#A7AABD");
            empty.Gravity = GravityFlags.Center;
            empty.SetPadding(Dp(12), Dp(48), Dp(12), Dp(48));
            agendaList.AddView(empty);
            return;
        }
        foreach (var task in tasks) agendaList.AddView(CreateAgendaItem(task));
    }

    void OpenDatePicker()
    {
        var picker = new DatePickerDialog(this, (_, args) =>
        {
            selectedDate = args.Date;
            Render();
        }, selectedDate.Year, selectedDate.Month - 1, selectedDate.Day);
        picker.SetTitle("Selecionar dia");
        picker.Show();
    }

    View CreateAgendaItem(TechnicianTask task)
    {
        var row = new LinearLayout(this) { Orientation = Orientation.Horizontal, Clickable = true, Focusable = true };
        row.SetGravity(GravityFlags.CenterVertical);
        row.SetPadding(Dp(14), Dp(15), Dp(14), Dp(15));
        row.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) { BottomMargin = Dp(10) };
        row.Background = Rounded("#191C24", StatusColor(task.Status), 10, 2);

        var time = Label($"{task.ScheduledStart:HH:mm}\n{task.ScheduledEnd:HH:mm}", 14, "#FFFFFF", true);
        time.Gravity = GravityFlags.Center;
        time.LayoutParameters = new LinearLayout.LayoutParams(Dp(58), ViewGroup.LayoutParams.MatchParent) { RightMargin = Dp(12) };
        row.AddView(time);

        var details = new LinearLayout(this) { Orientation = Orientation.Vertical };
        details.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1);
        details.AddView(Label($"{task.ReferenceCode}  ·  {task.Status}", 12, StatusColor(task.Status), true));
        details.AddView(Label(task.Title, 16, "#FFFFFF", true));
        details.AddView(Label($"⌖  {task.Location}  ·  {task.Priority}", 13, "#A7AABD"));
        row.AddView(details);
        row.Click += (_, _) => StartActivity(new Intent(this, typeof(TaskDetailActivity)).PutExtra("task_id", task.Id));
        return row;
    }

    TextView Label(string text, int size, string color, bool bold = false) { var label = new TextView(this) { Text = text, TextSize = size, Typeface = bold ? Typeface.DefaultBold : Typeface.Default }; label.SetTextColor(Color.ParseColor(color)); return label; }
    GradientDrawable Rounded(string fill, string stroke, int radius, int strokeWidth = 1) { var drawable = new GradientDrawable(); drawable.SetColor(Color.ParseColor(fill)); drawable.SetStroke(Dp(strokeWidth), Color.ParseColor(stroke)); drawable.SetCornerRadius(Dp(radius)); return drawable; }
    static string StatusColor(string status) => status switch { "Open" => "#0090E7", "In Progress" => "#FFAB00", "On Hold" => "#FFAB00", "Done" => "#00D25B", "Closed" => "#6C7293", _ => "#A7AABD" };
    int Dp(int value) => (int)(value * Resources!.DisplayMetrics!.Density + .5f);
}
