using Auth.Grpc;
using EmailService.Contracts;
using Flurl;
using MassTransit.Configuration;
using MediatR;
using Microsoft.Extensions.Options;
using Review.Application.Options;
using Review.Domain.Invitations;

namespace Review.Application.Features.Invitations.InviteReviewer;

public class InviteReviewerCommandHandler(
    ReviewDbContext _dbContext,
    ArticleRepository _articleRepository,
    ReviewerRepository _reviewerRepository,
    IOptions<EmailOptions> _emailOptions,
    IOptions<AppUrlsOptions> _appUrlsOptions,
    IEmailService _emailService,
    IPersonService _personClient) : IRequestHandler<InviteReviewerCommand, InviteReviewerResponse>
{
    public async Task<InviteReviewerResponse> Handle(InviteReviewerCommand command, CancellationToken ct)
    {
        var article = await _articleRepository.GetByIdOrThrowAsync(command.ArticleId);

        ReviewInvitation invitation = default!;

        if(command.UserId is not null)
        {
            var reviewer = await _reviewerRepository.GetByUserIdAsync(command.UserId.Value);
            if (reviewer is not null)
            {
                invitation = article.InviteReviewer(reviewer, command);
            }
            else
            {
                var response = await _personClient.GetPersonByUserIdAsync(
                    new GetPersonByUserIdRequest { UserId = command.UserId.Value });
                var personInfo = response.PersonInfo;

                invitation = article.InviteReviewer(personInfo.UserId, personInfo.Email, personInfo.FirstName, personInfo.LastName , command);
            }
        }
        else
        {
            invitation = article.InviteReviewer(command.UserId, command.Email, command.FirstName, command.LastnName, command);
        }

        await _dbContext.SaveChangesAsync(ct);

        var editor = await _dbContext.Editors.SingleAsync(e => e.UserId == command.CreatedById, ct);

        await _emailService.SendEmailAsync(BuildEmailMessage(article, invitation, editor), ct);

        return new InviteReviewerResponse(article.Id, invitation.Id, invitation.Token.Value);
    }

    private EmailMessage BuildEmailMessage(Article article, ReviewInvitation invitation, Editor editor)
    {
        const string InvitationEmail =
        @"Dear Contributor,<br/> 
            You've been invited by {0} to review the following article: {1}.<br/>
            Please accept or deny, the invitation will expire on {2}.<br/>
            If you don't have an account please create one using the following URL: {3}";

        var url =
        _appUrlsOptions.Value.ReviewUIBaseUrl
        .AppendPathSegment($"articles/{invitation.ArticleId}/invitations/{invitation.Token}");

        return new EmailMessage(
        "Review Invitation",
        new Content(ContentType.Html, string.Format(InvitationEmail, editor.FirstName + " " + editor.LastName, url, invitation.ExpiresOn, url)),
        new EmailAddress("articles", _emailOptions.Value.EmailFromAddress),
        new List<EmailAddress> { new EmailAddress(invitation.FullName, invitation.Email) }
        );
    }
}
