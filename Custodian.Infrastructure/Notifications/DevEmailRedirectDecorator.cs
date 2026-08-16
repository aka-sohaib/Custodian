using Custodian.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Custodian.Infrastructure.Notifications;

public class DevEmailRedirectDecorator : IEmailSender
{
    private readonly IEmailSender _innerEmailSender;
    private readonly EmailSettings _emailSettings;

    public DevEmailRedirectDecorator(ResendEmailSender innerEmailSender, IOptions<EmailSettings> emailSettings)
    {
        _innerEmailSender = innerEmailSender;
        _emailSettings    = emailSettings.Value;
    }

    public Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        //---- Redirect to DevEmail if DevRedirect is enabled ----
        if (_emailSettings.EnableDevRedirect && !string.IsNullOrWhiteSpace(_emailSettings.DevEmail))
        {
            string modifiedSubject = $"[DEV intended for: {toEmail}] {subject}";
            return _innerEmailSender.SendEmailAsync(_emailSettings.DevEmail, modifiedSubject, htmlBody, cancellationToken);
        }

        //---- Send directly if DevRedirect is disabled ----
        return _innerEmailSender.SendEmailAsync(toEmail, subject, htmlBody, cancellationToken);
    }
}
