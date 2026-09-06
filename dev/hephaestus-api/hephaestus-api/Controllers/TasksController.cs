using Hephaestus.Api.DTOs.Domain;
using Hephaestus.Api.Security;
using Hephaestus.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Api.Controllers;

[ApiController, Authorize, Route("api/tasks")]
public sealed class TasksController(TaskService service, HistoryService historyService) : DomainControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(CurrentUserId, CurrentRole, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> Get(int id, CancellationToken ct) =>
        Ok(await service.GetAsync(id, CurrentUserId, CurrentRole, ct));

    [HttpPost("/api/tickets/{ticketId:int}/tasks"), Authorize(Policy = Policies.TechnicalStaff)]
    public async Task<ActionResult<TaskResponse>> Create(int ticketId, CreateTaskRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(ticketId, request, CurrentUserId, CurrentRole, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}"), Authorize(Policy = Policies.TechnicalStaff)]
    public async Task<ActionResult<TaskResponse>> Update(int id, UpdateTaskRequest request, CancellationToken ct) =>
        Ok(await service.UpdateAsync(id, request, CurrentUserId, CurrentRole, ct));

    [HttpPatch("{id:int}/status"), Authorize(Policy = Policies.TechnicalStaff)]
    public async Task<ActionResult<TaskResponse>> ChangeStatus(int id, UpdateStatusRequest request, CancellationToken ct) =>
        Ok(await service.ChangeStatusAsync(id, request.StatusId, CurrentUserId, CurrentRole, ct));

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<IReadOnlyList<HistoryResponse>>> History(int id, CancellationToken ct) =>
        Ok(await historyService.GetAsync("Task", id, CurrentUserId, CurrentRole, ct));

    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<HistoryResponse>> Comment(int id, CreateCommentRequest request, CancellationToken ct) =>
        Ok(await historyService.AddCommentAsync("Task", id, request.Content, CurrentUserId, CurrentRole, ct));

    [HttpPost("{id:int}/attachments")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<HistoryResponse>> Attachment(int id, [FromForm] IFormFile file, CancellationToken ct) =>
        Ok(await historyService.AddAttachmentAsync("Task", id, file, CurrentUserId, CurrentRole, ct));
}
