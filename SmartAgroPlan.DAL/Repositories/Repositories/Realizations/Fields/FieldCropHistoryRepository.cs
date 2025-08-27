using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Fields;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Fields;

public class FieldCropHistoryRepository : RepositoryBase<FieldCropHistory>, IFieldCropHistoryRepository
{
    public FieldCropHistoryRepository(SmartAgroPlanDbContext dbContext) : base(dbContext)
    {
    }
}
