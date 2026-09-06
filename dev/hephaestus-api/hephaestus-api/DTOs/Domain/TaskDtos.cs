using System.ComponentModel.DataAnnotations;

namespace Hephaestus.Api.DTOs.Domain;

public sealed record TaskResponse(
    int Id, int TicketId, string ReferenceCode, int TechnicianId, string Technician,
    int StatusId, string Status, DateTime ScheduledStart, DateTime ScheduledEnd,
    DateTime? ActualStartDate, DateTime? ActualEndDate);

public sealed class CreateTaskRequest
{
    [Range(1, int.MaxValue)] public int TechnicianId { get; init; }
    public DateTime ScheduledStart { get; init; }
    public DateTime ScheduledEnd { get; init; }
}

public sealed class UpdateTaskRequest
{
    [Range(1, int.MaxValue)] public int TechnicianId { get; init; }
    public DateTime ScheduledStart { get; init; }
    public DateTime ScheduledEnd { get; init; }
    public DateTime? ActualStartDate { get; init; }
    public DateTime? ActualEndDate { get; init; }
}
