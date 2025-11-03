using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.FertilizerForecasting;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Realizations.FertilizerForecasting;

public class FertilizerApplicationRecordRepository : RepositoryBase<FertilizerApplicationRecord>,
    IFertilizerApplicationRecordRepository
{
    public FertilizerApplicationRecordRepository(SmartAgroPlanDbContext context) : base(context)
    {
    }
}
