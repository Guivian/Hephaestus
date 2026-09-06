using System.ComponentModel.DataAnnotations;

namespace Hephaestus.Api.DTOs.Domain;

public sealed record TicketResponse(
    int Id, string ReferenceCode, string TicketType, string Title, string Description,
    int PriorityId, string Priority, int StatusId, string Status,
    int CreatedById, string CreatedBy, int? AssignedToId, string? AssignedTo, DateTime OpenDate);

public sealed class CreateTicketRequest
{
    [Required, StringLength(10)] public string TicketType { get; init; } = string.Empty;
    [Required, StringLength(200)] public string Title { get; init; } = string.Empty;
    [Required, StringLength(4000)] public string Description { get; init; } = string.Empty;
    [Range(1, int.MaxValue)] public int PriorityId { get; init; }
    public int? AssignedToId { get; init; }
}

public sealed class UpdateTicketRequest
{
    [Required, StringLength(10)] public string TicketType { get; init; } = string.Empty;
    [Required, StringLength(200)] public string Title { get; init; } = string.Empty;
    [Required, StringLength(4000)] public string Description { get; init; } = string.Empty;
    [Range(1, int.MaxValue)] public int PriorityId { get; init; }
    public int? AssignedToId { get; init; }
}

public sealed class UpdateStatusRequest
{
    [Range(1, int.MaxValue)] public int StatusId { get; init; }
}
