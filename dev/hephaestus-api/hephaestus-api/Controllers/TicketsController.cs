using Hephaestus.Api.DTOs.Domain;
using Hephaestus.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Api.Controllers;

[ApiController, Authorize, Route("api/tickets")]
public sealed class TicketsController(TicketService service, HistoryService historyService) : DomainControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketResponse>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(CurrentUserId, CurrentRole, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponse>> Get(int id, CancellationToken ct) =>
        Ok(await service.GetAsync(id, CurrentUserId, CurrentRole, ct));

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> Create(CreateTicketRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, CurrentUserId, CurrentRole, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketResponse>> Update(int id, UpdateTicketRequest request, CancellationToken ct) =>
        Ok(await service.UpdateAsync(id, request, CurrentUserId, CurrentRole, ct));

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TicketResponse>> ChangeStatus(int id, UpdateStatusRequest request, CancellationToken ct) =>
        Ok(await service.ChangeStatusAsync(id, request.StatusId, CurrentUserId, CurrentRole, ct));

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<IReadOnlyList<HistoryResponse>>> History(int id, CancellationToken ct) =>
        Ok(await historyService.GetAsync("Ticket", id, CurrentUserId, CurrentRole, ct));

    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<HistoryResponse>> Comment(int id, CreateCommentRequest request, CancellationToken ct) =>
        Ok(await historyService.AddCommentAsync("Ticket", id, request.Content, CurrentUserId, CurrentRole, ct));

    [HttpPost("{id:int}/attachments")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<HistoryResponse>> Attachment(int id, [FromForm] IFormFile file, CancellationToken ct) =>
        Ok(await historyService.AddAttachmentAsync("Ticket", id, file, CurrentUserId, CurrentRole, ct));
}
