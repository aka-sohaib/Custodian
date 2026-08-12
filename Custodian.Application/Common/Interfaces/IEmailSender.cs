namespace Custodian.Application.Common.Interfaces;

public interface IEmailSender
{
    Task SendInvitationEmailAsync(string toEmail, string acceptURL, CancellationToken cancellationToken);
}
