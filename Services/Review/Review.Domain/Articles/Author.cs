using Review.Domain.Shared;

namespace Review.Domain.Articles;

public class Author : Person
{
    public string? Degree { get; init; }
    public string? Discipline { get; init; }
}
