using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;

namespace SmartAgroPlan.DAL.Configurations;

public class FertilizationPlanMap : IEntityTypeConfiguration<FertilizationPlan>
{
    public void Configure(EntityTypeBuilder<FertilizationPlan> builder)
    {
        builder.HasMany(fp => fp.Stages)
            .WithOne(ps => ps.FertilizationPlan)
            .HasForeignKey(ps => ps.FertilizationPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
