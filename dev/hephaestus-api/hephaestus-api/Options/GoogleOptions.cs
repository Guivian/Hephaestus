
namespace Hephaestus.Api.Options;

public sealed class GoogleOptions
{
    public const string SectionName = "Authentication:Google";

    public string WebClientId { get; set; } = string.Empty;
    public string WebClientSecret { get; set; } = string.Empty;
    public string AndroidClientId { get; set; } = string.Empty;
}
