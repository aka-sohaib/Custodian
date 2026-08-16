namespace Custodian.Application.Common.Interfaces;

public interface IEmailSender
{
    //---- Send single unified email ----
    Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
