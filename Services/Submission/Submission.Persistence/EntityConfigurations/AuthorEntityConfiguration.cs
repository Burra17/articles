using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission.Domain.Entities;

namespace Submission.Persistence.EntityConfigurations;

internal class AuthorEntityConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.Property(e => e.Discipline).HasMaxLength(64)
            .HasComment("The scientific discipline of the author, e.g., Computer Science, Biology, etc.");

        builder.Property(e => e.Degree).HasMaxLength(64)
            .HasComment("The highest academic degree of the author, e.g., PhD, MD, etc.");
    }
}
