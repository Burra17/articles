using Blocks.EntityFrameworkCore.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission.Domain.Entities;

namespace Submission.Persistence.EntityConfigurations;

internal class PersonEntityConfiguration : EntityConfiguration<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasDiscriminator(e => e.TypeDiscriminator)
            .HasValue<Person>(nameof(Person))
            .HasValue<Author>(nameof(Author));

        builder.Property(e => e.FirstName).HasMaxLength(64).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(64).IsRequired();
        builder.Property(e => e.Title).HasMaxLength(64);
        builder.Property(e => e.Affiliation).HasMaxLength(512).IsRequired()
            .HasComment("Institution or organization they are associated with when they conduct their research");
        builder.Property(e => e.UserId).IsRequired(false);

        builder.ComplexProperty(
            o => o.EmailAddress, builder =>
            {
                builder.Property(n => n.Value)
                    .HasColumnName(builder.Metadata.PropertyInfo!.Name)
                    .HasMaxLength(64);
            });
    }
}
