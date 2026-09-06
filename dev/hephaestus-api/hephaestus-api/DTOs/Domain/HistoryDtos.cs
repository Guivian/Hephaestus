using System.ComponentModel.DataAnnotations;

namespace Hephaestus.Api.DTOs.Domain;

public sealed record HistoryResponse(int Id, string RecordType, string? Content, int UserId, string User, DateTime CreatedDate);

public sealed class CreateCommentRequest
{
    [Required, StringLength(4000)] public string Content { get; init; } = string.Empty;
}

public sealed record AttachmentMetadata(string StoredName, string OriginalName, string ContentType, long Size);
