namespace Hephaestus.Api.Options;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";
    public string? RootPath { get; init; }
    public long MaximumBytes { get; init; } = 10 * 1024 * 1024;
}
