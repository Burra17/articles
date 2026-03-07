using Auth.Domain.Users.Events;
using EmailService.Contracts;
using Microsoft.Extensions.Options;
using Blocks.AspNetCore;
using Flurl;

namespace Auth.API.Features.Users.CreateUser;

public class SendConfirmationEmailOnUserCreatedHandler
    (IEmailService emailService, IHttpContextAccessor httpContextAccessor, IOptions<EmailOptions> emailOptions)
    : IEventHandler<UserCreated>
{
    public async Task HandleAsync(UserCreated eventModel, CancellationToken ct)
    {
        var url = httpContextAccessor.HttpContext?.Request.BaseUrl()
            .AppendPathSegment("password")
            .SetQueryParams(new { eventModel.ResetPasswordToken });

        var emailMessage = BuildConfirmationEmail(eventModel.User, url, emailOptions.Value.EmailFromAddress);
        await emailService.SendEmailAsync(emailMessage, ct);
    }

    public EmailMessage BuildConfirmationEmail(User user, string resetLink, string fromEmailAddress)
    {
        const string ConfirmationEmail =
            "Dear {0}, <br/>An Account has been created for you. <br/>Please set your password using the following URL: <br/>{1}";

        return new EmailMessage(
            "Your account has been created - Set your password",
            new Content(ContentType.Html, string.Format(ConfirmationEmail, user.Person.FirstName, resetLink)),
            new EmailAddress("Articles", fromEmailAddress),
            new List<EmailAddress> { new EmailAddress(user.Person.FirstName, user.Email!) }
            );
    }
}
