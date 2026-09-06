namespace Hephaestus.Api.Models;

public sealed class Priority
{
    public int PrioritiesId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TicketType { get; set; } = string.Empty;
}

public sealed class TicketStatus
{
    public int TicketStatusesId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class Ticket
{
    public int TicketsId { get; set; }
    public string ReferenceCode { get; set; } = string.Empty;
    public string TicketType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PrioritiesId { get; set; }
    public int TicketStatusesId { get; set; }
    public int CreatedById { get; set; }
    public int? AssignedToId { get; set; }
    public DateTime OpenDate { get; set; }
    public Priority Priority { get; set; } = null!;
    public TicketStatus Status { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public User? AssignedTo { get; set; }
}

public sealed class WorkTaskStatus
{
    public int TaskStatusesId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class WorkTask
{
    public int TasksId { get; set; }
    public int TicketsId { get; set; }
    public int TechnicianId { get; set; }
    public string ReferenceCode { get; set; } = string.Empty;
    public int TaskStatusesId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public User Technician { get; set; } = null!;
    public WorkTaskStatus Status { get; set; } = null!;
}

public sealed class AttachmentAndHistory
{
    public int AttachmentsAndHistoryId { get; set; }
    public int ReferenceId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string RecordType { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int UsersId { get; set; }
    public DateTime CreatedDate { get; set; }
    public User User { get; set; } = null!;
}
