using Custodian.Application.Common.Interfaces;
using System.Net.Http.Json;
using Resend;
using Custodian.Application.Common.Exceptions;

namespace Custodian.Infrastructure.Notifications;

public class ResendEmailSender: IEmailSender
{
    private readonly IResend _resend;
    public ResendEmailSender(IResend resend) => _resend = resend;

    public async Task SendInvitationEmailAsync(string toEmail, string acceptURL, CancellationToken cancellationToken)
    {
        //---- Make the payload according to resends' required structure ----
        var message = new EmailMessage
        {
            From = "Custodian <onboarding@resend.dev>",
            To = new[] { toEmail },
            Subject = "You have been invited to join Custodian",
            HtmlBody = $@"
                <h2>Welcome to Custodian!</h2>
                <p>You have been invited to join the platform.</p>
                <p>Click <a href='{acceptURL}'>here</a> to accept your invitation and set up your account.</p>
                <br/>
                <p>If you did not expect this email, you can safely ignore it.</p>"
        };

        //---- Send the email through Resend and get response ----
        try
        {
            await _resend.EmailSendAsync(message, cancellationToken);
        }
        catch(Exception ex)
        {
            throw new ExternalServiceException($"Failed to send email via Resend SDK: {ex.Message}");
        }
    }
}
