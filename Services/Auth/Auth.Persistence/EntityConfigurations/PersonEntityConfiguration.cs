using Auth.Domain.Persons;
using Blocks.Core.Constraints;
using Blocks.EntityFrameworkCore.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Blocks.EntityFrameworkCore;
using Auth.Domain.Persons.ValueObjects;

namespace Auth.Persistence.EntityConfigurations;

internal class PersonEntityConfiguration : EntityConfiguration<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(MaxLength.C64);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(MaxLength.C64);
        builder.Property(e => e.Gender).IsRequired().HasEnumConversion();

        builder.OwnsOne(
            e => e.Email, b =>
            {
                b.Property(n => n.Value)
                .HasColumnName(nameof(Person.Email))
                .HasMaxLength(MaxLength.C64);
                b.Property(e => e.NormalizedEmail)
                .HasColumnName(nameof(EmailAddress.NormalizedEmail))
                .HasMaxLength(MaxLength.C64);

                b.HasIndex(e => e.NormalizedEmail).IsUnique();

            });

        builder.OwnsOne(
            e => e.Honorific, b =>
            {
                b.Property(e => e.Value).HasMaxLength(MaxLength.C32)
                .HasColumnName(nameof(Person.Honorific));

                b.WithOwner(); // required to avoid navigation issues
            });

        builder.OwnsOne(e => e.ProfessionalProfile, b =>
        {
            b.Property(e => e.Position).HasMaxLength(MaxLength.C32).HasColumnNameSameAsProperty();
            b.Property(e => e.CompanyName).HasMaxLength(MaxLength.C32).HasColumnNameSameAsProperty();
            b.Property(e => e.Affiliation).HasMaxLength(MaxLength.C32).HasColumnNameSameAsProperty();

            b.WithOwner(); // required to avoid navigation issues
        });

        builder.Property(e => e.PictureUrl).HasMaxLength(MaxLength.C2048);
    }
}