using EmailService.Contracts;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailService.Smtp;

public class SmtpEmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    public SmtpEmailService(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }
    public async Task<bool> SendEmailAsync(EmailMessage emailMessage, CancellationToken ct)
    {
        var message = emailMessage.ToMailKitMessage();

        using var smtpClient = new SmtpClient();

        try
        {
            await smtpClient.ConnectAsync(_emailOptions.Smtp.Host, _emailOptions.Smtp.Port, _emailOptions.Smtp.UseSSL, ct);
            await smtpClient.AuthenticateAsync(_emailOptions.Smtp.Username, _emailOptions.Smtp.Password, ct);
            await smtpClient.SendAsync(message, ct);
        }
        catch (Exception ex)
        {
            // todo log and throw

            return false;
        }

        return true;
    }
}
