using Blocks.Domain.Entities;
using Submission.Domain.ValueObjects;

namespace Submission.Domain.Entities;

public class Person : IEntity
{
    public int Id { get; init; }

    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public string FullName => FirstName + " " + LastName;

    public string? Title { get; set; }

    public required EmailAddress EmailAddress { get; set; }

    public required string Affiliation { get; set; }

    public int? UserId { get; init; }

    public IReadOnlyList<ArticleActor> ArticleActors { get; private set; } = new List<ArticleActor>();

    public string TypeDiscriminator { get; init; } = null!; //Ef Core discriminator 
}
