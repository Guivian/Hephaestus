using System.Text.Json;
using Hephaestus.Api.DTOs.Domain;
using Hephaestus.Api.Models;
using Hephaestus.Api.Options;
using Hephaestus.Api.Repositories;
using Hephaestus.Api.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hephaestus.Api.Services;

public sealed class HistoryService(IDomainRepository repository, IOptions<FileStorageOptions> options)
{
    private static readonly Dictionary<string, string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = "application/pdf", [".png"] = "image/png",
        [".jpg"] = "image/jpeg", [".jpeg"] = "image/jpeg"
    };
    private readonly FileStorageOptions settings = options.Value;

    public async Task<IReadOnlyList<HistoryResponse>> GetAsync(string entityType, int referenceId, int actorId, string role, CancellationToken cancellationToken)
    {
        await EnsureAccessAsync(entityType, referenceId, actorId, role, false, cancellationToken);
        return await repository.QueryHistory().AsNoTracking()
            .Where(x => x.EntityType == entityType && x.ReferenceId == referenceId)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new HistoryResponse(x.AttachmentsAndHistoryId, x.RecordType, x.Content, x.UsersId, x.User.Name, x.CreatedDate))
            .ToListAsync(cancellationToken);
    }

    public async Task<HistoryResponse> AddCommentAsync(string entityType, int referenceId, string content, int actorId, string role, CancellationToken cancellationToken)
    {
        await EnsureAccessAsync(entityType, referenceId, actorId, role, true, cancellationToken);
        var user = await repository.FindUserAsync(actorId, cancellationToken)
            ?? throw new DomainException(StatusCodes.Status401Unauthorized, "Utilizador inválido.");
        var record = new AttachmentAndHistory
        {
            ReferenceId = referenceId, EntityType = entityType, RecordType = "Comment",
            Content = content.Trim(), UsersId = actorId, User = user, CreatedDate = DateTime.UtcNow
        };
        repository.AddHistory(record);
        await repository.SaveChangesAsync(cancellationToken);
        return new(record.AttachmentsAndHistoryId, record.RecordType, record.Content, actorId, user.Name, record.CreatedDate);
    }

    public async Task<HistoryResponse> AddAttachmentAsync(string entityType, int referenceId, IFormFile file, int actorId, string role, CancellationToken cancellationToken)
    {
        await EnsureAccessAsync(entityType, referenceId, actorId, role, true, cancellationToken);
        ValidateFile(file);
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        await using (var input = file.OpenReadStream()) await ValidateSignatureAsync(input, extension, cancellationToken);

        var root = GetStorageRoot();
        Directory.CreateDirectory(root);
        var storedName = $"{Guid.NewGuid():N}{extension}";
        var destination = Path.Combine(root, storedName);
        await using (var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            await file.CopyToAsync(output, cancellationToken);

        try
        {
            var user = await repository.FindUserAsync(actorId, cancellationToken)
                ?? throw new DomainException(StatusCodes.Status401Unauthorized, "Utilizador inválido.");
            var metadata = new AttachmentMetadata(storedName, Path.GetFileName(file.FileName), AllowedTypes[extension], file.Length);
            var record = new AttachmentAndHistory
            {
                ReferenceId = referenceId, EntityType = entityType, RecordType = "Attachment",
                Content = JsonSerializer.Serialize(metadata), UsersId = actorId, User = user, CreatedDate = DateTime.UtcNow
            };
            repository.AddHistory(record);
            await repository.SaveChangesAsync(cancellationToken);
            return new(record.AttachmentsAndHistoryId, record.RecordType, record.Content, actorId, user.Name, record.CreatedDate);
        }
        catch
        {
            File.Delete(destination);
            throw;
        }
    }

    public async Task<(string Path, AttachmentMetadata Metadata)> GetAttachmentAsync(int id, int actorId, string role, CancellationToken cancellationToken)
    {
        var record = await repository.FindHistoryAsync(id, cancellationToken);
        if (record is null || record.RecordType != "Attachment")
            throw new DomainException(StatusCodes.Status404NotFound, "Anexo não encontrado.");
        await EnsureAccessAsync(record.EntityType, record.ReferenceId, actorId, role, false, cancellationToken);
        var metadata = JsonSerializer.Deserialize<AttachmentMetadata>(record.Content ?? string.Empty)
            ?? throw new DomainException(StatusCodes.Status500InternalServerError, "Metadados do anexo inválidos.");
        var path = Path.Combine(GetStorageRoot(), metadata.StoredName);
        if (!File.Exists(path)) throw new DomainException(StatusCodes.Status404NotFound, "Ficheiro do anexo não encontrado.");
        return (path, metadata);
    }

    private async Task EnsureAccessAsync(string entityType, int referenceId, int actorId, string role, bool write, CancellationToken cancellationToken)
    {
        if (entityType == "Ticket")
        {
            var ticket = await repository.FindTicketAsync(referenceId, cancellationToken)
                ?? throw new DomainException(StatusCodes.Status404NotFound, "Ticket não encontrado.");
            TicketService.EnsureCanView(ticket, actorId, role);
            if (write && role == RoleNames.Technician && ticket.AssignedToId != actorId)
                throw new DomainException(StatusCodes.Status403Forbidden, "Não pode adicionar registos a este ticket.");
            return;
        }
        if (entityType == "Task")
        {
            var task = await repository.FindTaskAsync(referenceId, cancellationToken)
                ?? throw new DomainException(StatusCodes.Status404NotFound, "Tarefa não encontrada.");
            if (role == RoleNames.Standard && task.Ticket.CreatedById != actorId ||
                role == RoleNames.Technician && task.TechnicianId != actorId)
                throw new DomainException(StatusCodes.Status403Forbidden, "Não tem acesso a esta tarefa.");
            return;
        }
        throw new DomainException(StatusCodes.Status400BadRequest, "Tipo de entidade inválido.");
    }

    private void ValidateFile(IFormFile file)
    {
        if (file.Length <= 0) throw new DomainException(StatusCodes.Status400BadRequest, "O ficheiro está vazio.");
        if (file.Length > settings.MaximumBytes) throw new DomainException(StatusCodes.Status413PayloadTooLarge, "O ficheiro excede o tamanho máximo permitido.");
        var extension = Path.GetExtension(file.FileName);
        if (!AllowedTypes.TryGetValue(extension, out var expected) || !file.ContentType.Equals(expected, StringComparison.OrdinalIgnoreCase))
            throw new DomainException(StatusCodes.Status415UnsupportedMediaType, "Apenas PDF, PNG e JPEG são permitidos e o MIME type tem de corresponder.");
    }

    private static async Task ValidateSignatureAsync(Stream stream, string extension, CancellationToken cancellationToken)
    {
        var buffer = new byte[8];
        var read = await stream.ReadAsync(buffer, cancellationToken);
        var valid = extension switch
        {
            ".pdf" => read >= 4 && buffer[..4].SequenceEqual("%PDF"u8),
            ".png" => read >= 8 && buffer.SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            ".jpg" or ".jpeg" => read >= 3 && buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF,
            _ => false
        };
        if (!valid) throw new DomainException(StatusCodes.Status415UnsupportedMediaType, "O conteúdo do ficheiro não corresponde à extensão.");
    }

    private string GetStorageRoot() => Path.GetFullPath(settings.RootPath ??
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Hephaestus", "uploads"));
}
