using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;

namespace SmartAgroPlan.DAL.Configurations;

public class FertilizerApplicationRecordProductMap : IEntityTypeConfiguration<FertilizerApplicationRecordProduct>
{
    public void Configure(EntityTypeBuilder<FertilizerApplicationRecordProduct> builder)
    {
        builder.HasOne(x => x.FertilizerProduct)
            .WithMany()
            .HasForeignKey(x => x.FertilizerProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ApplicationRecord)
            .WithMany()
            .HasForeignKey(x => x.ApplicationRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
