using Hephaestus.Api.DTOs.Domain;
using Hephaestus.Api.Models;
using Hephaestus.Api.Repositories;
using Hephaestus.Api.Security;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Api.Services;

public sealed class TicketService(IDomainRepository repository)
{
    public async Task<IReadOnlyList<TicketResponse>> GetAllAsync(int actorId, string role, CancellationToken cancellationToken)
    {
        var query = repository.QueryTickets().AsNoTracking();
        if (role == RoleNames.Standard)
            query = query.Where(x => x.CreatedById == actorId);

        var tickets = await query.OrderByDescending(x => x.OpenDate).ToListAsync(cancellationToken);
        return tickets.Select(Map).ToList();
    }

    public async Task<TicketResponse> GetAsync(int id, int actorId, string role, CancellationToken cancellationToken)
    {
        var ticket = await repository.FindTicketAsync(id, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status404NotFound, "Ticket não encontrado.");
        EnsureCanView(ticket, actorId, role);
        return Map(ticket);
    }

    public async Task<TicketResponse> CreateAsync(CreateTicketRequest request, int actorId, string role, CancellationToken cancellationToken)
    {
        var priority = await ValidatePriorityAsync(request.PriorityId, request.TicketType, cancellationToken);
        var assignedTo = await ValidateAssignmentAsync(request.AssignedToId, actorId, role, cancellationToken);
        var openStatus = await repository.FindTicketStatusAsync(1, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status500InternalServerError, "O estado inicial Open não existe.");

        var ticket = new Ticket
        {
            ReferenceCode = $"TKT-{DateTime.UtcNow:yyMMdd}-{Guid.NewGuid():N}"[..19].ToUpperInvariant(),
            TicketType = request.TicketType.Trim().ToUpperInvariant(),
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            PrioritiesId = priority.PrioritiesId,
            TicketStatusesId = openStatus.TicketStatusesId,
            CreatedById = actorId,
            AssignedToId = assignedTo?.UsersId,
            OpenDate = DateTime.UtcNow,
            Priority = priority,
            Status = openStatus,
            CreatedBy = (await repository.FindUserAsync(actorId, cancellationToken))!,
            AssignedTo = assignedTo
        };
        repository.AddTicket(ticket);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(ticket);
    }

    public async Task<TicketResponse> UpdateAsync(int id, UpdateTicketRequest request, int actorId, string role, CancellationToken cancellationToken)
    {
        var ticket = await repository.FindTicketAsync(id, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status404NotFound, "Ticket não encontrado.");
        EnsureCanEdit(ticket, actorId, role);
        EnsureNotClosed(ticket);

        var priority = await ValidatePriorityAsync(request.PriorityId, request.TicketType, cancellationToken);
        var assignedTo = await ValidateAssignmentAsync(request.AssignedToId, actorId, role, cancellationToken);
        ticket.TicketType = request.TicketType.Trim().ToUpperInvariant();
        ticket.Title = request.Title.Trim();
        ticket.Description = request.Description.Trim();
        ticket.PrioritiesId = priority.PrioritiesId;
        ticket.Priority = priority;
        ticket.AssignedToId = assignedTo?.UsersId;
        ticket.AssignedTo = assignedTo;
        await repository.SaveChangesAsync(cancellationToken);
        return Map(ticket);
    }

    public async Task<TicketResponse> ChangeStatusAsync(int id, int statusId, int actorId, string role, CancellationToken cancellationToken)
    {
        var ticket = await repository.FindTicketAsync(id, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status404NotFound, "Ticket não encontrado.");
        EnsureCanChangeStatus(ticket, actorId, role);
        EnsureNotClosed(ticket);
        var status = await repository.FindTicketStatusAsync(statusId, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status400BadRequest, "Estado de ticket inválido.");
        ticket.TicketStatusesId = status.TicketStatusesId;
        ticket.Status = status;
        await repository.SaveChangesAsync(cancellationToken);
        return Map(ticket);
    }

    public static void EnsureCanView(Ticket ticket, int actorId, string role)
    {
        if (role == RoleNames.Standard && ticket.CreatedById != actorId)
            throw new DomainException(StatusCodes.Status403Forbidden, "Não tem acesso a este ticket.");
    }

    private static void EnsureCanEdit(Ticket ticket, int actorId, string role)
    {
        var allowed = role is RoleNames.Admin or RoleNames.Manager ||
            role == RoleNames.Standard && ticket.CreatedById == actorId ||
            role == RoleNames.Technician && ticket.AssignedToId == actorId;
        if (!allowed) throw new DomainException(StatusCodes.Status403Forbidden, "Não pode alterar este ticket.");
    }

    private static void EnsureCanChangeStatus(Ticket ticket, int actorId, string role)
    {
        var allowed = role is RoleNames.Admin or RoleNames.Manager ||
            role == RoleNames.Technician && ticket.AssignedToId == actorId;
        if (!allowed) throw new DomainException(StatusCodes.Status403Forbidden, "Não pode alterar o estado deste ticket.");
    }

    private static void EnsureNotClosed(Ticket ticket)
    {
        if (ticket.Status.Name.Equals("Closed", StringComparison.OrdinalIgnoreCase))
            throw new DomainException(StatusCodes.Status409Conflict, "Tickets fechados não podem ser alterados.");
    }

    private async Task<Priority> ValidatePriorityAsync(int priorityId, string ticketType, CancellationToken cancellationToken)
    {
        var normalizedType = ticketType.Trim().ToUpperInvariant();
        if (normalizedType is not ("SUP" or "SVC"))
            throw new DomainException(StatusCodes.Status400BadRequest, "TicketType deve ser SUP ou SVC.");
        var priority = await repository.FindPriorityAsync(priorityId, cancellationToken);
        if (priority is null || !priority.TicketType.Equals(normalizedType, StringComparison.OrdinalIgnoreCase))
            throw new DomainException(StatusCodes.Status400BadRequest, "A prioridade não é válida para o tipo de ticket.");
        return priority;
    }

    private async Task<User?> ValidateAssignmentAsync(int? userId, int actorId, string role, CancellationToken cancellationToken)
    {
        if (!userId.HasValue) return null;
        if (role == RoleNames.Standard || role == RoleNames.Technician && userId != actorId)
            throw new DomainException(StatusCodes.Status403Forbidden, "Não pode atribuir o ticket a esse utilizador.");
        var user = await repository.FindUserAsync(userId.Value, cancellationToken);
        if (user is null || !user.IsActive || user.Role.Name != RoleNames.Technician)
            throw new DomainException(StatusCodes.Status400BadRequest, "O responsável tem de ser um técnico ativo.");
        return user;
    }

    private static TicketResponse Map(Ticket x) => new(x.TicketsId, x.ReferenceCode, x.TicketType, x.Title,
        x.Description, x.PrioritiesId, x.Priority.Code, x.TicketStatusesId, x.Status.Name,
        x.CreatedById, x.CreatedBy.Name, x.AssignedToId, x.AssignedTo?.Name, x.OpenDate);
}
