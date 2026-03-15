using EmailService.Contracts;
using MassTransit.Configuration;
using MediatR;
using Microsoft.Extensions.Options;
using Review.Domain.Articles.Events;
using Review.Domain.Reviewers;

namespace Review.Application.Features.Invitations.AcceptInvitation;

public class SendConfirmationEmailOnReviewerAssignedHandler(IEmailService _emailService, IOptions<EmailOptions> _emailOptions)
    : INotificationHandler<ReviewerAssigned>
{
    public async Task Handle(ReviewerAssigned notification, CancellationToken ct)
    {
        await _emailService.SendEmailAsync(
            BuildEmailMessage(notification.Article, notification.Reviewer, notification.Article.Editor), ct);
    }

    private EmailMessage BuildEmailMessage(Article article, Reviewer reviewer, Editor editor)
    {
        const string EmailBody =
                @"Dear Editor, The reviewer {0} has been assigned the following article: {1}.";

        return new EmailMessage(
            "Reviewer Assigned",
            new Content(ContentType.Html, string.Format(EmailBody, reviewer.FirstName + " " + reviewer.LastName, article.Title)),
            new EmailAddress("articles", _emailOptions.Value.EmailFromAddress),
            new List<EmailAddress> { new EmailAddress(editor.FullName, editor.Email) }
            );
    }
}
