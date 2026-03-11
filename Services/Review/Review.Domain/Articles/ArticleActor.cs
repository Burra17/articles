using Articles.Abstractions.Enums;
using Auth.Domain.Persons;

namespace Review.Domain.Articles;

public class ArticleActor
{
    public int ArticleId { get; init; }
    public Article Article { get; init; } = null!;

    public int PersonId { get; init; }
    public Person Person { get; init; } = null!;

    public UserRoleType Role { get; init; }
}
