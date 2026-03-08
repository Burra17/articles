namespace Journals.API.Features.Journals.Shared;

public record JournalDto(
    int Id,
    string Abbrevation,
    string Name,
    string Description,
    string ISSN)
{
    public EditorDto Editor { get; set; } = null!;
}