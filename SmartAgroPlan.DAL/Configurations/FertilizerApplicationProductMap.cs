using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Applications;

namespace SmartAgroPlan.DAL.Configurations;

public class FertilizerApplicationProductMap : IEntityTypeConfiguration<FertilizerApplicationProduct>
{
    public void Configure(EntityTypeBuilder<FertilizerApplicationProduct> builder)
    {
        builder.HasOne(x => x.ApplicationPlan)
            .WithMany(ap => ap.Products)
            .HasForeignKey(x => x.ApplicationPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.FertilizerProduct)
            .WithMany(fp => fp.ApplicationProducts)
            .HasForeignKey(x => x.FertilizerProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
