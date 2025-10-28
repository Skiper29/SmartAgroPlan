using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;

namespace SmartAgroPlan.DAL.Configurations;

public class FertilizerApplicationRecordProductMap : IEntityTypeConfiguration<FertilizerApplicationRecordProduct>
{
    public void Configure(EntityTypeBuilder<FertilizerApplicationRecordProduct> builder)
    {
        builder.HasOne(x => x.FertilizerProduct)
            .WithMany(fp => fp.RecordProducts)
            .HasForeignKey(x => x.FertilizerProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ApplicationRecord)
            .WithMany(ar => ar.ProductsUsed)
            .HasForeignKey(x => x.ApplicationRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
