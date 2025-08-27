using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Fields;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Fields;

public class SoilRepository : RepositoryBase<Soil>, ISoilRepository
{
    public SoilRepository(SmartAgroPlanDbContext dbContext) : base(dbContext)
    {
    }
}
