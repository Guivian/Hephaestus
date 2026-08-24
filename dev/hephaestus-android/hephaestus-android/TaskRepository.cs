namespace hephaestus_android;

internal sealed class TechnicianTask
{
    public required int Id { get; init; }
    public required string ReferenceCode { get; init; }
    public required string TicketReference { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Status { get; set; }
    public required string Priority { get; init; }
    public required string Location { get; init; }
    public required string Equipment { get; init; }
    public required string Technician { get; init; }
    public required DateTime ScheduledStart { get; init; }
    public required DateTime ScheduledEnd { get; init; }
    public string? ActualStart { get; set; }
    public string? ActualEnd { get; set; }
}

internal sealed class SupportTicket
{
    public required int Id { get; init; }
    public required string ReferenceCode { get; init; }
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Status { get; init; }
    public required string Priority { get; init; }
    public required string Location { get; init; }
    public required string Equipment { get; init; }
    public required string CreatedBy { get; init; }
}

internal sealed record ActivityEntry(
    string EntityReference,
    bool IsComment,
    string Author,
    string Text,
    DateTime Timestamp);

// Fonte temporária para o frontend. Substituir por um serviço HTTP quando a API existir.
internal static class TaskRepository
{
    static readonly List<SupportTicket> Tickets = [];
    static readonly List<TechnicianTask> Tasks = [];
    static readonly List<ActivityEntry> Activity = [];

    public static IReadOnlyList<SupportTicket> AllTickets => Tickets;
    public static IReadOnlyList<TechnicianTask> All => Tasks;
    public static TechnicianTask? Find(int id) => Tasks.FirstOrDefault(task => task.Id == id);
    public static SupportTicket? FindTicket(int id) => Tickets.FirstOrDefault(ticket => ticket.Id == id);
    public static SupportTicket? FindTicket(string reference) => Tickets.FirstOrDefault(ticket => ticket.ReferenceCode == reference);
    public static IReadOnlyList<TechnicianTask> ForTicket(string reference) => Tasks.Where(task => task.TicketReference == reference).ToList();
    public static IReadOnlyList<ActivityEntry> CommentsFor(string reference) => Activity.Where(entry => entry.EntityReference == reference && entry.IsComment).OrderByDescending(entry => entry.Timestamp).ToList();
    public static IReadOnlyList<ActivityEntry> HistoryFor(string reference) => Activity.Where(entry => entry.EntityReference == reference && !entry.IsComment).OrderByDescending(entry => entry.Timestamp).ToList();
    public static IReadOnlyList<ActivityEntry> RecentActivity(IEnumerable<string> references) => Activity
        .Where(entry => references.Contains(entry.EntityReference))
        .OrderByDescending(entry => entry.Timestamp)
        .ToList();

    public static bool UpdateStatus(int id, string status)
    {
        var task = Find(id);
        if (task is null || task.Status == "Closed") return false;
        var expectedNext = task.Status switch { "Open" => "In Progress", "In Progress" => "Done", _ => null };
        if (status != expectedNext) return false;
        var previous = task.Status;
        task.Status = status;
        if (status == "In Progress" && task.ActualStart is null) task.ActualStart = DateTime.Now.ToString("HH:mm");
        if (status == "Done" && task.ActualEnd is null) task.ActualEnd = DateTime.Now.ToString("HH:mm");
        Activity.Add(new(task.ReferenceCode, false, task.Technician, $"Estado alterado de {previous} para {status}.", DateTime.Now));
        return true;
    }
}
