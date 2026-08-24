using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace hephaestus_android;

[Activity(Label = "Detalhe da tarefa", Exported = false)]
public class TaskDetailActivity : Activity
{
    TechnicianTask task = null!;
    TextView status = null!;
    TextView actualStart = null!;
    TextView actualEnd = null!;

    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        if (!SessionStore.IsSignedIn(this))
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
            return;
        }

        task = TaskRepository.Find(Intent?.GetIntExtra("task_id", -1) ?? -1)!;
        if (task is null) { Finish(); return; }
        SetContentView(Resource.Layout.activity_task_detail);
        status = FindViewById<TextView>(Resource.Id.detailStatus)!;
        actualStart = FindViewById<TextView>(Resource.Id.detailActualStart)!;
        actualEnd = FindViewById<TextView>(Resource.Id.detailActualEnd)!;

        FindViewById<TextView>(Resource.Id.detailReference)!.Text = task.ReferenceCode;
        FindViewById<TextView>(Resource.Id.detailTitle)!.Text = task.Title;
        FindViewById<TextView>(Resource.Id.detailDescription)!.Text = task.Description;
        var ticketLink = FindViewById<TextView>(Resource.Id.detailTicket)!;
        ticketLink.Text = $"Ticket de origem  ·  {task.TicketReference}  ›";
        ticketLink.Click += (_, _) =>
        {
            var ticket = TaskRepository.FindTicket(task.TicketReference);
            if (ticket is not null) StartActivity(new Intent(this, typeof(TicketDetailActivity)).PutExtra("ticket_id", ticket.Id));
        };
        FindViewById<TextView>(Resource.Id.detailSchedule)!.Text = $"{task.ScheduledStart:dddd, dd MMMM}\n{task.ScheduledStart:HH:mm} – {task.ScheduledEnd:HH:mm}";
        FindViewById<TextView>(Resource.Id.detailLocation)!.Text = task.Location;
        FindViewById<TextView>(Resource.Id.detailEquipment)!.Text = task.Equipment;
        FindViewById<TextView>(Resource.Id.detailTechnician)!.Text = task.Technician;
        FindViewById<Button>(Resource.Id.detailBack)!.Click += (_, _) => Finish();
        FindViewById<Button>(Resource.Id.statusOpen)!.Click += (_, _) => ChangeStatus("Open");
        FindViewById<Button>(Resource.Id.statusProgress)!.Click += (_, _) => ChangeStatus("In Progress");
        FindViewById<Button>(Resource.Id.statusDone)!.Click += (_, _) => ChangeStatus("Done");
        FindViewById<Button>(Resource.Id.taskCommentsViewAll)!.Click += (_, _) => OpenActivityFeed(true);
        FindViewById<Button>(Resource.Id.taskHistoryViewAll)!.Click += (_, _) => OpenActivityFeed(false);
        RenderStatus();
        RenderActivity();
        BottomNavigation.Setup(this, NavigationTab.Tasks);
    }

    void ChangeStatus(string next)
    {
        if (task.Status == next) return;
        var dialog = new AlertDialog.Builder(this);
        dialog.SetTitle("Atualizar estado");
        dialog.SetMessage($"Alterar {task.ReferenceCode} de {task.Status} para {next}?");
        dialog.SetNegativeButton("Cancelar", (_, _) => { });
        dialog.SetPositiveButton("Confirmar", (_, _) =>
        {
            if (!TaskRepository.UpdateStatus(task.Id, next)) return;
            RenderStatus();
            RenderActivity();
            Toast.MakeText(this, $"Estado atualizado para {next}", ToastLength.Short)!.Show();
        });
        dialog.Show();
    }

    void RenderStatus()
    {
        status.Text = task.Status;
        var color = task.Status switch { "Open" => "#0090E7", "In Progress" => "#FFAB00", "Done" => "#00D25B", _ => "#A7AABD" };
        var fill = task.Status switch { "Open" => "#132B3A", "In Progress" => "#332B12", "Done" => "#123021", _ => "#242833" };
        status.SetTextColor(Color.ParseColor(color));
        status.Background = RoundedBackground(fill, color);
        actualStart.Text = task.ActualStart ?? "—";
        actualEnd.Text = task.ActualEnd ?? "—";
        FindViewById<Button>(Resource.Id.statusOpen)!.Visibility = ViewStates.Gone;
        FindViewById<Button>(Resource.Id.statusProgress)!.Visibility = task.Status == "Open" ? ViewStates.Visible : ViewStates.Gone;
        FindViewById<Button>(Resource.Id.statusDone)!.Visibility = task.Status == "In Progress" ? ViewStates.Visible : ViewStates.Gone;
        FindViewById<TextView>(Resource.Id.taskCompletedMessage)!.Visibility = task.Status == "Done" ? ViewStates.Visible : ViewStates.Gone;
    }

    void RenderActivity()
    {
        ActivityFeedRenderer.Render(this, FindViewById<LinearLayout>(Resource.Id.taskComments)!, TaskRepository.CommentsFor(task.ReferenceCode).Take(2).ToList(), "Ainda não existem comentários.");
        ActivityFeedRenderer.Render(this, FindViewById<LinearLayout>(Resource.Id.taskHistory)!, TaskRepository.HistoryFor(task.ReferenceCode).Take(2).ToList(), "Ainda não existe histórico.");
    }

    void OpenActivityFeed(bool comments) => StartActivity(new Intent(this, typeof(ActivityFeedActivity))
        .PutExtra("reference", task.ReferenceCode)
        .PutExtra("comments", comments));

    GradientDrawable RoundedBackground(string fill, string stroke)
    {
        var drawable = new GradientDrawable();
        drawable.SetColor(Color.ParseColor(fill));
        drawable.SetStroke((int)Resources!.DisplayMetrics!.Density, Color.ParseColor(stroke));
        drawable.SetCornerRadius(20 * Resources.DisplayMetrics.Density);
        return drawable;
    }
}
