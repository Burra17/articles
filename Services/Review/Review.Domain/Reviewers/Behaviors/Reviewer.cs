using Articles.Abstractions;
using Auth.Grpc;
using Blocks.Core.Extensions;
using Blocks.Domain;
using Review.Domain.Articles;
using Review.Domain.Reviewers.Events;
using Review.Domain.Shared.ValueObjects;

namespace Review.Domain.Reviewers;

public partial class Reviewer
{
    public static Reviewer Create(PersonInfo personInfo, IEnumerable<int> journalIds, IArticleAction action)
    {
        var reviewer = new Reviewer
        {
            Id = personInfo.Id,
            UserId = personInfo.UserId,
            Email = EmailAddress.Create(personInfo.Email),
            FirstName = personInfo.FirstName,
            LastName = personInfo.LastName,
            Honorific = personInfo.Honorific,
            Affiliation = personInfo.ProfessionalProfile!.Affiliation,
        };

        if (journalIds.IsNullOrEmpty())
            reviewer._specializations
                = [.. journalIds.Select(journalId => new ReviewerSpecialization { JournalId = journalId, ReviewerId = reviewer.Id })];

        else
            throw new DomainException("Reviewer must have at least one specialization");

        var domainEvent = new ReviewerCreated(reviewer, action);
        reviewer.AddDomainEvent(domainEvent);

        return reviewer;
    }

    public void AddSpecialization(ReviewerSpecialization specialization)
    {
        if (!_specializations.Contains(specialization))
            _specializations.Add(specialization);
    }
}
