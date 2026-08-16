namespace Custodian.Infrastructure.Notifications;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string DevEmail { get; set; } = string.Empty;
    public bool EnableDevRedirect { get; set; } = true;
    public string FrontendBaseUrl { get; set; } = "http://localhost:3000";
}
