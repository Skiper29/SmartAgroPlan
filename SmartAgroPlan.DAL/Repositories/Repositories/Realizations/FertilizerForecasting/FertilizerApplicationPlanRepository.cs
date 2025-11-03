using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Applications;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.FertilizerForecasting;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Realizations.FertilizerForecasting;

public class FertilizerApplicationPlanRepository : RepositoryBase<FertilizerApplicationPlan>,
    IFertilizerApplicationPlanRepository
{
    public FertilizerApplicationPlanRepository(SmartAgroPlanDbContext context) : base(context)
    {
    }
}
