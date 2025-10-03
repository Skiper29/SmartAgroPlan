using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.DAL.Configurations;

public class FieldMap : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.HasOne(f => f.Soil)
            .WithMany(s => s.Fields)
            .HasForeignKey(f => f.SoilId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.CurrentCrop)
            .WithMany(c => c.Fields)
            .HasForeignKey(f => f.CurrentCropId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.CropHistories)
            .WithOne(ch => ch.Field)
            .HasForeignKey(ch => ch.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Conditions)
            .WithOne(fc => fc.Field)
            .HasForeignKey(fc => fc.FieldId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
