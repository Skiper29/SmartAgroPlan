using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Persistence.Converters;

namespace SmartAgroPlan.DAL.Configurations;

public class CropVarietyMap : IEntityTypeConfiguration<CropVariety>
{
    public void Configure(EntityTypeBuilder<CropVariety> builder)
    {
        builder.Property(builder => builder.SowingStart)
               .HasConversion<DayMonthConverter>();

        builder.Property(builder => builder.SowingEnd)
                .HasConversion<DayMonthConverter>();

        builder.HasOne(cv => cv.OptimalSoil)
               .WithMany(s => s.OptimalCrops)
               .HasForeignKey(cv => cv.OptimalSoilId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
