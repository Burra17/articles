

using Blocks.Core.Extensions;

namespace Submission.Domain.ValueObjects;

public class FileExtensions
{
    public IReadOnlyList<string> Extensions { get; init; } = null!;

    public bool IsValidExstension(string exstension)
        // note: if the list  is empty, it means all extensions are allowed
        => Extensions.IsEmpty() || Extensions.Contains(exstension, StringComparer.OrdinalIgnoreCase);
}
