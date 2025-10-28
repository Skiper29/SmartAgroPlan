using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;

namespace SmartAgroPlan.DAL.Configurations;

public class PlanStageMap : IEntityTypeConfiguration<PlanStage>
{
    public void Configure(EntityTypeBuilder<PlanStage> builder)
    {
        const int precision = 5;
        const int scale = 4;

        builder.Property(s => s.NitrogenPercent).HasPrecision(precision, scale);
        builder.Property(s => s.PhosphorusPercent).HasPrecision(precision, scale);
        builder.Property(s => s.PotassiumPercent).HasPrecision(precision, scale);
        builder.Property(s => s.SulfurPercent).HasPrecision(precision, scale);
        builder.Property(s => s.CalciumPercent).HasPrecision(precision, scale);
        builder.Property(s => s.MagnesiumPercent).HasPrecision(precision, scale);
        builder.Property(s => s.BoronPercent).HasPrecision(precision, scale);
        builder.Property(s => s.ZincPercent).HasPrecision(precision, scale);
        builder.Property(s => s.ManganesePercent).HasPrecision(precision, scale);
        builder.Property(s => s.CopperPercent).HasPrecision(precision, scale);
        builder.Property(s => s.IronPercent).HasPrecision(precision, scale);
        builder.Property(s => s.MolybdenumPercent).HasPrecision(precision, scale);
    }
}
