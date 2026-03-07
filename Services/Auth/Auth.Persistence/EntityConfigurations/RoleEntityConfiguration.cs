using Auth.Domain.Roles;
using Blocks.Core.Constraints;
using Blocks.EntityFrameworkCore;
using Blocks.EntityFrameworkCore.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Persistence.EntityConfigurations;

internal class RoleEntityConfiguration : EntityConfiguration<Role>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Type).HasEnumConversion().IsRequired();
        builder.Property(e => e.Description).IsRequired().HasMaxLength(MaxLength.C256);
    }
}
