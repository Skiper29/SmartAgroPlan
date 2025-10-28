using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting;

namespace SmartAgroPlan.DAL.Configurations;

public class FertilizerProductMap : IEntityTypeConfiguration<FertilizerProduct>
{
    public void Configure(EntityTypeBuilder<FertilizerProduct> builder)
    {
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Type);
    }
}
