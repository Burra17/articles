using Articles.Abstractions.Enums;

namespace ArticleHub.Domain.Articles;

public class ArticleActor
{
    public int ArticleId { get; init; }
    public int PersonId { get; init; }
    public Person Person { get; init; } = null!;

    public UserRoleType Role {  get; init; }
}
