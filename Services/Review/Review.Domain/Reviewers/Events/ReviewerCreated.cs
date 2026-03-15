using Articles.Abstractions;
using Blocks.Domain;

namespace Review.Domain.Reviewers.Events;

public record ReviewerCreated(Reviewer author, IArticleAction action) : IDomainEvent;