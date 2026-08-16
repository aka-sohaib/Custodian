using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Resend;

namespace Custodian.Infrastructure.Notifications;

public class ResendEmailSender : IEmailSender
{
    private readonly IResend _resend;

    public ResendEmailSender(IResend resend) => _resend = resend;

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        //---- Build Resend email payload ----
        var message = new EmailMessage
        {
            From = "Custodian <onboarding@resend.dev>",
            To = new[] { toEmail },
            Subject = subject,
            HtmlBody = htmlBody
        };

        //---- Send email via Resend SDK ----
        try
        {
            await _resend.EmailSendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ExternalServiceException($"Failed to send email via Resend SDK: {ex.Message}");
        }
    }
}
