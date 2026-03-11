using Blocks.Core.Constraints;

namespace Review.Persistence.EntityConfigurations;

internal class AuthorEntityConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasBaseType<Person>();

        builder.Property(e => e.Discipline).HasMaxLength(MaxLength.C64)
            .HasComment("The scientific discipline of the author, e.g., Computer Science, Biology, etc.");

        builder.Property(e => e.Degree).HasMaxLength(MaxLength.C512)
            .HasComment("The highest academic degree of the author, e.g., PhD, MD, etc.");
    }
}
