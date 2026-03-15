using Blocks.Domain.Entities;

namespace ArticleHub.Domain.Articles;

public class Person : Entity
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string FullName => FirstName + " " + LastName;

    public required string Email {  get; init; }

    public int? UserId { get; set; }

    public string? Honorific {  get; init; }
}
