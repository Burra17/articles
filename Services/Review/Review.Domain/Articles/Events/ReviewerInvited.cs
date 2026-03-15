using Blocks.Domain;
using Review.Domain.Invitations;

namespace Review.Domain.Articles.Events;

public record ReviewerInvited(Article article, ReviewInvitation Invitation): IDomainEvent;
