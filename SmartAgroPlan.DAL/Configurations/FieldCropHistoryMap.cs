using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.DAL.Configurations;

public class FieldCropHistoryMap : IEntityTypeConfiguration<FieldCropHistory>
{
    public void Configure(EntityTypeBuilder<FieldCropHistory> builder)
    {
        builder.HasOne(fch => fch.Field)
               .WithMany(f => f.CropHistories)
               .HasForeignKey(fch => fch.FieldId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fch => fch.Crop)
               .WithMany(c => c.FieldCropHistories)
               .HasForeignKey(fch => fch.CropId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
