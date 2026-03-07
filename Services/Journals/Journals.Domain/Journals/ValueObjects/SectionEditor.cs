using Journals.Domain.Journals.Enums;
using Redis.OM.Modeling;

namespace Journals.Domain.Journals.ValueObjects;

[Document]
public class SectionEditor
{
    public int EditorId { get; init; }

    public EditorRole EditorRole { get; set; }
}
