
using Blocks.Core.Constraints;

namespace ArticleHub.Persistence.EntityConfigurations;

internal class JournalEntityConfiguration : EntityConfiguration<Journal>
{
    protected override bool HasGeneratedId => false;

    public override void Configure(EntityTypeBuilder<Journal> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Abbreviation).HasMaxLength(MaxLength.C8).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(MaxLength.C64).IsRequired();
    }
}
