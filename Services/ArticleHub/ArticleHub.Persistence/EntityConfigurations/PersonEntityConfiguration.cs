
using Blocks.Core.Constraints;

namespace ArticleHub.Persistence.EntityConfigurations;

internal class PersonEntityConfiguration : EntityConfiguration<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.UserId).IsRequired(false);
        builder.Property(e => e.FirstName).HasMaxLength(MaxLength.C64).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(MaxLength.C64).IsRequired();
        builder.Property(e => e.Honorific).HasMaxLength(MaxLength.C64).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(MaxLength.C256).IsRequired();
    }
}
