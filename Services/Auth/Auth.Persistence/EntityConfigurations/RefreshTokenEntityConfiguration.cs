using Auth.Domain.Users;
using Blocks.Core.Constraints;
using Blocks.EntityFrameworkCore.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Persistence.EntityConfigurations;

internal class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Token).IsRequired().HasMaxLength(MaxLength.C512);
        builder.Property(e => e.CreatedByIp).HasMaxLength(MaxLength.C128);
        builder.Property(e => e.RevokedByIp).HasMaxLength(MaxLength.C128);
    }
}
