using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;

namespace SmartAgroPlan.DAL.Configurations;

public class FertilizerApplicationRecordMap : IEntityTypeConfiguration<FertilizerApplicationRecord>
{
    public void Configure(EntityTypeBuilder<FertilizerApplicationRecord> builder)
    {
        builder.HasOne(x => x.Field)
            .WithMany()
            .HasForeignKey(x => x.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ApplicationMethod)
            .WithMany()
            .HasForeignKey(x => x.ApplicationMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.FieldId, x.ApplicationDate });
        builder.HasIndex(x => x.RecordedDate);
    }
}
