using Hephaestus.Api.Models;

namespace Hephaestus.Api.Repositories;

public interface IDomainRepository
{
    IQueryable<Ticket> QueryTickets();
    IQueryable<WorkTask> QueryTasks();
    IQueryable<AttachmentAndHistory> QueryHistory();
    Task<Ticket?> FindTicketAsync(int id, CancellationToken cancellationToken);
    Task<WorkTask?> FindTaskAsync(int id, CancellationToken cancellationToken);
    Task<Priority?> FindPriorityAsync(int id, CancellationToken cancellationToken);
    Task<TicketStatus?> FindTicketStatusAsync(int id, CancellationToken cancellationToken);
    Task<WorkTaskStatus?> FindTaskStatusAsync(int id, CancellationToken cancellationToken);
    Task<User?> FindUserAsync(int id, CancellationToken cancellationToken);
    Task<AttachmentAndHistory?> FindHistoryAsync(int id, CancellationToken cancellationToken);
    Task<bool> TechnicianHasConflictAsync(int technicianId, DateTime start, DateTime end, int? excludedTaskId, CancellationToken cancellationToken);
    void AddTicket(Ticket ticket);
    void AddTask(WorkTask task);
    void AddHistory(AttachmentAndHistory record);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
