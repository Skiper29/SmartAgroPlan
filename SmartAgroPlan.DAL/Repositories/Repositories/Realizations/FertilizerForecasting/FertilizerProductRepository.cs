using SmartAgroPlan.DAL.Entities.FertilizerForecasting;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.FertilizerForecasting;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Realizations.FertilizerForecasting;

public class FertilizerProductRepository : RepositoryBase<FertilizerProduct>, IFertilizerProductRepository
{
    public FertilizerProductRepository(SmartAgroPlanDbContext context) : base(context)
    {
    }
}
