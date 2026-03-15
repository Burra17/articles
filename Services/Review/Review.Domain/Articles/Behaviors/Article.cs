using Articles.Abstractions;
using Articles.Abstractions.Enums;
using Articles.Abstractions.Events.Dtos;
using Auth.Grpc;
using Blocks.Domain;
using Review.Domain.Articles.Events;
using Review.Domain.Assets;
using Review.Domain.Invitations;
using Review.Domain.Invitations.ValueObjects;
using Review.Domain.Reviewers;

namespace Review.Domain.Articles;

public partial class Article
{
    public static Article AcceptSubmitted(ArticleDto articleDto, IEnumerable<ArticleActor> actors, IEnumerable<Asset> assets)
    {
        var article = new Article
        {
            Id = articleDto.Id,
            JournalId = articleDto.Journal.Id,
            Title = articleDto.Title,
            Type = articleDto.Type,
            Scope = articleDto.Scope,
            SubmittedById = articleDto.SubmittedBy.Id,
            SubmittedOn = articleDto.SubmittedOn,
            Stage = articleDto.Stage,
        };
        article._actors.AddRange(actors);
        article._assets.AddRange(assets);

        return article;
    }

    public ReviewInvitation InviteReviewer(Reviewer reviewer, IArticleAction action)
    {
        if (!reviewer.Specializations.Any(s => s.JournalId == this.JournalId))
            throw new DomainException($"Reviewer {reviewer.FullName} is not specialized in articles journal.");

        return CreateInvitation(reviewer.UserId,  reviewer.Email.Value, reviewer.FirstName, reviewer.LastName, action: action);
    }

    public ReviewInvitation InviteReviewer(int? userId, string email, string firstName, string lastName, IArticleAction action)
    {
        return CreateInvitation(userId, email, firstName, lastName, action);
    }

    private ReviewInvitation CreateInvitation(int? userId, string email, string firstName, string lastName, IArticleAction action)
    {
        if (_invitations.Any(i =>
            i.Email.Value.Trim().ToUpperInvariant() == email.Trim().ToUpperInvariant()
            && !i.IsExpired))
            throw new DomainException($"Reviewer {firstName} {lastName} {email} was already invited.");

        var invitation = new ReviewInvitation
        {
            ArticleId = this.Id,
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            SentById = action.CreatedById,
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            Token = InvitationToken.CreateNew(),
        };

        AddDomainEvent(new ReviewerInvited(this, invitation));

        _invitations.Add(invitation);

        return invitation;
    }

    public void AssignReviewer(Reviewer reviewer, IArticleAction action)
    {
        if (_actors.Exists(a => a.PersonId == reviewer.Id && a.Role == UserRoleType.REV))
            throw new DomainException($"Reviewer {reviewer.Email} is already assigned to the article");

        reviewer.AddSpecialization(new ReviewerSpecialization() { JournalId = this.JournalId, ReviewerId = reviewer.Id });

        _actors.Add(new ArticleActor() { PersonId = reviewer.Id, Role = UserRoleType.REV });

        AddDomainEvent(
            new ReviewerAssigned(this, reviewer, action));
    }
}
