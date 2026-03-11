using Blocks.Core.Constraints;
using Review.Domain.Assets;

namespace Review.Persistence.EntityConfigurations;

internal class AssetEntityConfiguration : EntityConfiguration<Asset>
{
    public override void Configure(EntityTypeBuilder<Asset> builder)
    {
        base.Configure(builder);

        builder.ComplexProperty(
             e => e.Name, builder =>
             {
                 builder.Property(n => n.Value)
                     .HasColumnName(builder.Metadata.PropertyInfo!.Name)
                     .HasMaxLength(MaxLength.C64).IsRequired();
             });

        builder.Property(e => e.State).HasEnumConversion().IsRequired();
        builder.Property(e => e.Type).HasEnumConversion().IsRequired();

        builder.HasOne(d => d.Article).WithMany(p => p.Assets)
            .HasForeignKey(d => d.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.TypeDefinition).WithMany()
            .HasForeignKey(e => e.Type)
            .HasPrincipalKey(e => e.Name)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);


        builder.ComplexProperty(e => e.File, fileBuilder =>
        {
            new FileEntityConfiguration().Configure(fileBuilder);
        });
    }
}