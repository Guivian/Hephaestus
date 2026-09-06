
namespace Hephaestus.Api.Security;

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Manager = "Gestor";
    public const string Technician = "Técnico";
    public const string Standard = "Standard";
}

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string Management = "Management";
    public const string TechnicalStaff = "TechnicalStaff";
    public const string StandardOnly = "StandardOnly";
}
