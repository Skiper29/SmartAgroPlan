using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.FertilizerForecasting;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Realizations.FertilizerForecasting;

public class FertilizationPlanRepository : RepositoryBase<FertilizationPlan>, IFertilizationPlanRepository
{
    public FertilizationPlanRepository(SmartAgroPlanDbContext context) : base(context)
    {
    }
}
