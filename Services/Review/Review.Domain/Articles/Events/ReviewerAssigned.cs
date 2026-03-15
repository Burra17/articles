using Articles.Abstractions;
using Blocks.Domain;
using Review.Domain.Reviewers;

namespace Review.Domain.Articles.Events;

public record ReviewerAssigned(Article Article, Reviewer Reviewer, IArticleAction action) : IDomainEvent;
