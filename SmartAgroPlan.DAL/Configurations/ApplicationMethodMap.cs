using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting;

namespace SmartAgroPlan.DAL.Configurations;

public class ApplicationMethodMap : IEntityTypeConfiguration<ApplicationMethod>
{
    public void Configure(EntityTypeBuilder<ApplicationMethod> builder)
    {
        builder.HasMany(am => am.PlanStages)
            .WithOne(ps => ps.ApplicationMethod)
            .HasForeignKey(ps => ps.ApplicationMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new ApplicationMethod
            {
                Id = 1, Name = "Розкидання з загортанням",
                Description = "Внесення твердих добрив перед сівбою з подальшим загортанням у ґрунт."
            },
            new ApplicationMethod
            {
                Id = 2, Name = "Прикореневе підживлення",
                Description = "Внесення добрив у ґрунт поблизу кореневої зони під час вегетації."
            },
            new ApplicationMethod
            {
                Id = 3, Name = "Поверхневе підживлення (розкидання)",
                Description = "Розкидання добрив по поверхні ґрунту (часто для азоту)."
            },
            new ApplicationMethod
            {
                Id = 4, Name = "Листкове обприскування",
                Description = "Внесення розчинених добрив безпосередньо на листя рослини."
            },
            new ApplicationMethod
            {
                Id = 5, Name = "Фертигація",
                Description = "Внесення добрив разом із поливною водою через систему зрошення."
            }
        );
    }
}
