using Hephaestus.Api.Data;
using Hephaestus.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Api.Repositories;

public sealed class DomainRepository(HephaestusDbContext database) : IDomainRepository
{
    public IQueryable<Ticket> QueryTickets() => database.Tickets
        .Include(x => x.Priority).Include(x => x.Status).Include(x => x.CreatedBy).Include(x => x.AssignedTo);

    public IQueryable<WorkTask> QueryTasks() => database.Tasks
        .Include(x => x.Ticket).ThenInclude(x => x.CreatedBy)
        .Include(x => x.Ticket).ThenInclude(x => x.Status)
        .Include(x => x.Technician).Include(x => x.Status);

    public IQueryable<AttachmentAndHistory> QueryHistory() => database.AttachmentsAndHistory.Include(x => x.User);

    public Task<Ticket?> FindTicketAsync(int id, CancellationToken cancellationToken) =>
        QueryTickets().SingleOrDefaultAsync(x => x.TicketsId == id, cancellationToken);

    public Task<WorkTask?> FindTaskAsync(int id, CancellationToken cancellationToken) =>
        QueryTasks().SingleOrDefaultAsync(x => x.TasksId == id, cancellationToken);

    public Task<Priority?> FindPriorityAsync(int id, CancellationToken cancellationToken) =>
        database.Priorities.SingleOrDefaultAsync(x => x.PrioritiesId == id, cancellationToken);

    public Task<TicketStatus?> FindTicketStatusAsync(int id, CancellationToken cancellationToken) =>
        database.TicketStatuses.SingleOrDefaultAsync(x => x.TicketStatusesId == id, cancellationToken);

    public Task<WorkTaskStatus?> FindTaskStatusAsync(int id, CancellationToken cancellationToken) =>
        database.TaskStatuses.SingleOrDefaultAsync(x => x.TaskStatusesId == id, cancellationToken);

    public Task<User?> FindUserAsync(int id, CancellationToken cancellationToken) =>
        database.Users.Include(x => x.Role).SingleOrDefaultAsync(x => x.UsersId == id, cancellationToken);

    public Task<AttachmentAndHistory?> FindHistoryAsync(int id, CancellationToken cancellationToken) =>
        QueryHistory().SingleOrDefaultAsync(x => x.AttachmentsAndHistoryId == id, cancellationToken);

    public Task<bool> TechnicianHasConflictAsync(int technicianId, DateTime start, DateTime end, int? excludedTaskId, CancellationToken cancellationToken) =>
        database.Tasks.AnyAsync(x => x.TechnicianId == technicianId &&
            (!excludedTaskId.HasValue || x.TasksId != excludedTaskId.Value) &&
            x.TaskStatusesId != 5 && start < x.ScheduledEnd && end > x.ScheduledStart, cancellationToken);

    public void AddTicket(Ticket ticket) => database.Tickets.Add(ticket);
    public void AddTask(WorkTask task) => database.Tasks.Add(task);
    public void AddHistory(AttachmentAndHistory record) => database.AttachmentsAndHistory.Add(record);
    public Task SaveChangesAsync(CancellationToken cancellationToken) => database.SaveChangesAsync(cancellationToken);
}
