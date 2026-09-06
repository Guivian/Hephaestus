using Hephaestus.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Api.Controllers;

[ApiController, Authorize, Route("api/attachments")]
public sealed class AttachmentsController(HistoryService service) : DomainControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Download(int id, CancellationToken ct)
    {
        var attachment = await service.GetAttachmentAsync(id, CurrentUserId, CurrentRole, ct);
        return PhysicalFile(attachment.Path, attachment.Metadata.ContentType, attachment.Metadata.OriginalName);
    }
}
