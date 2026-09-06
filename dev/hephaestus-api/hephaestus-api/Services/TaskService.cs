using Hephaestus.Api.DTOs.Domain;
using Hephaestus.Api.Models;
using Hephaestus.Api.Repositories;
using Hephaestus.Api.Security;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Api.Services;

public sealed class TaskService(IDomainRepository repository)
{
    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(int actorId, string role, CancellationToken cancellationToken)
    {
        var query = repository.QueryTasks().AsNoTracking();
        if (role == RoleNames.Standard)
            query = query.Where(x => x.Ticket.CreatedById == actorId);
        else if (role == RoleNames.Technician)
            query = query.Where(x => x.TechnicianId == actorId);
        var tasks = await query.OrderBy(x => x.ScheduledStart).ToListAsync(cancellationToken);
        return tasks.Select(Map).ToList();
    }

    public async Task<TaskResponse> GetAsync(int id, int actorId, string role, CancellationToken cancellationToken)
    {
        var task = await repository.FindTaskAsync(id, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status404NotFound, "Tarefa não encontrada.");
        EnsureCanView(task, actorId, role);
        return Map(task);
    }

    public async Task<TaskResponse> CreateAsync(int ticketId, CreateTaskRequest request, int actorId, string role, CancellationToken cancellationToken)
    {
        var ticket = await repository.FindTicketAsync(ticketId, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status404NotFound, "Ticket não encontrado.");
        EnsureTicketOpen(ticket);
        ValidateSchedule(request.ScheduledStart, request.ScheduledEnd);
        var technician = await ValidateTechnicianAsync(request.TechnicianId, actorId, role, cancellationToken);
        await EnsureNoConflictAsync(request.TechnicianId, request.ScheduledStart, request.ScheduledEnd, null, cancellationToken);
        var status = await repository.FindTaskStatusAsync(1, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status500InternalServerError, "O estado inicial Open não existe.");

        var task = new WorkTask
        {
            TicketsId = ticketId,
            TechnicianId = technician.UsersId,
            ReferenceCode = $"TSK-{DateTime.UtcNow:yyMMdd}-{Guid.NewGuid():N}"[..19].ToUpperInvariant(),
            TaskStatusesId = status.TaskStatusesId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = request.ScheduledEnd,
            Ticket = ticket,
            Technician = technician,
            Status = status
        };
        repository.AddTask(task);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(task);
    }

    public async Task<TaskResponse> UpdateAsync(int id, UpdateTaskRequest request, int actorId, string role, CancellationToken cancellationToken)
    {
        var task = await repository.FindTaskAsync(id, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status404NotFound, "Tarefa não encontrada.");
        EnsureCanEdit(task, actorId, role);
        EnsureTaskOpen(task);
        EnsureTicketOpen(task.Ticket);
        ValidateSchedule(request.ScheduledStart, request.ScheduledEnd);
        if (request.ActualStartDate.HasValue && request.ActualEndDate < request.ActualStartDate)
            throw new DomainException(StatusCodes.Status400BadRequest, "ActualEndDate não pode ser anterior a ActualStartDate.");
        var technician = await ValidateTechnicianAsync(request.TechnicianId, actorId, role, cancellationToken);
        await EnsureNoConflictAsync(request.TechnicianId, request.ScheduledStart, request.ScheduledEnd, id, cancellationToken);

        task.TechnicianId = technician.UsersId;
        task.Technician = technician;
        task.ScheduledStart = request.ScheduledStart;
        task.ScheduledEnd = request.ScheduledEnd;
        task.ActualStartDate = request.ActualStartDate;
        task.ActualEndDate = request.ActualEndDate;
        await repository.SaveChangesAsync(cancellationToken);
        return Map(task);
    }

    public async Task<TaskResponse> ChangeStatusAsync(int id, int statusId, int actorId, string role, CancellationToken cancellationToken)
    {
        var task = await repository.FindTaskAsync(id, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status404NotFound, "Tarefa não encontrada.");
        EnsureCanEdit(task, actorId, role);
        EnsureTaskOpen(task);
        EnsureTicketOpen(task.Ticket);
        var status = await repository.FindTaskStatusAsync(statusId, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status400BadRequest, "Estado de tarefa inválido.");
        task.TaskStatusesId = status.TaskStatusesId;
        task.Status = status;
        var now = DateTime.UtcNow;
        if (status.Name == "In Progress" && !task.ActualStartDate.HasValue) task.ActualStartDate = now;
        if (status.Name is "Done" or "Closed" && !task.ActualEndDate.HasValue) task.ActualEndDate = now;
        await repository.SaveChangesAsync(cancellationToken);
        return Map(task);
    }

    private async Task<User> ValidateTechnicianAsync(int id, int actorId, string role, CancellationToken cancellationToken)
    {
        if (role == RoleNames.Technician && id != actorId)
            throw new DomainException(StatusCodes.Status403Forbidden, "Um técnico só pode atribuir tarefas a si próprio.");
        var technician = await repository.FindUserAsync(id, cancellationToken);
        if (technician is null || !technician.IsActive || technician.Role.Name != RoleNames.Technician)
            throw new DomainException(StatusCodes.Status400BadRequest, "É necessário indicar um técnico ativo.");
        return technician;
    }

    private async Task EnsureNoConflictAsync(int id, DateTime start, DateTime end, int? excludedId, CancellationToken cancellationToken)
    {
        if (await repository.TechnicianHasConflictAsync(id, start, end, excludedId, cancellationToken))
            throw new DomainException(StatusCodes.Status409Conflict, "O técnico já tem uma tarefa marcada nesse intervalo.");
    }

    private static void ValidateSchedule(DateTime start, DateTime end)
    {
        if (start == default || end == default || end <= start)
            throw new DomainException(StatusCodes.Status400BadRequest, "ScheduledEnd tem de ser posterior a ScheduledStart.");
    }

    private static void EnsureCanView(WorkTask task, int actorId, string role)
    {
        if (role == RoleNames.Standard && task.Ticket.CreatedById != actorId ||
            role == RoleNames.Technician && task.TechnicianId != actorId)
            throw new DomainException(StatusCodes.Status403Forbidden, "Não tem acesso a esta tarefa.");
    }

    private static void EnsureCanEdit(WorkTask task, int actorId, string role)
    {
        if (role is not (RoleNames.Admin or RoleNames.Manager) &&
            !(role == RoleNames.Technician && task.TechnicianId == actorId))
            throw new DomainException(StatusCodes.Status403Forbidden, "Não pode alterar esta tarefa.");
    }

    private static void EnsureTicketOpen(Ticket ticket)
    {
        if (ticket.Status.Name.Equals("Closed", StringComparison.OrdinalIgnoreCase))
            throw new DomainException(StatusCodes.Status409Conflict, "Não é possível alterar tarefas de um ticket fechado.");
    }

    private static void EnsureTaskOpen(WorkTask task)
    {
        if (task.Status.Name.Equals("Closed", StringComparison.OrdinalIgnoreCase))
            throw new DomainException(StatusCodes.Status409Conflict, "Tarefas fechadas não podem ser alteradas.");
    }

    private static TaskResponse Map(WorkTask x) => new(x.TasksId, x.TicketsId, x.ReferenceCode,
        x.TechnicianId, x.Technician.Name, x.TaskStatusesId, x.Status.Name, x.ScheduledStart,
        x.ScheduledEnd, x.ActualStartDate, x.ActualEndDate);
}
