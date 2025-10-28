using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Applications;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;

namespace SmartAgroPlan.DAL.Configurations;

public class FertilizerApplicationPlanMap : IEntityTypeConfiguration<FertilizerApplicationPlan>
{
    public void Configure(EntityTypeBuilder<FertilizerApplicationPlan> builder)
    {
        builder.HasOne(x => x.Field)
            .WithMany()
            .HasForeignKey(x => x.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CompletionRecord)
            .WithOne(x => x.ApplicationPlan)
            .HasForeignKey<FertilizerApplicationRecord>(x => x.ApplicationPlanId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.FertilizationPlan)
            .WithMany()
            .HasForeignKey(x => x.FertilizationPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlanStage)
            .WithMany()
            .HasForeignKey(x => x.PlanStageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.FieldId, x.PlannedApplicationDate });
        builder.HasIndex(x => x.IsCompleted);
    }
}
