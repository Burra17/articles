using Journals.Domain.Journals.Enums;

namespace Journals.API.Features.Journals.Shared;

public record EditorDto(
    int Id,
    string Fullname,
    string Affiliation,
    EditorRole Role = EditorRole.ChiefEditor);
