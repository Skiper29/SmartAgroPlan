using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Crops;

public class CropVarietyRepository : RepositoryBase<CropVariety>, ICropVarietyRepository
{
    public CropVarietyRepository(SmartAgroPlanDbContext dbContext) : base(dbContext)
    {
    }
}
