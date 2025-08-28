using System.Transactions;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Fields;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Fields;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly SmartAgroPlanDbContext _dbContext;
    private IFieldRepository? _fieldRepository;
    private ICropVarietyRepository? _cropVarietyRepository;
    private ISoilRepository? _soilRepository;
    private IFieldCropHistoryRepository? _fieldCropHistoryRepository;

    public RepositoryWrapper(SmartAgroPlanDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IFieldRepository FieldRepository =>
        _fieldRepository ??= new FieldRepository(_dbContext);

    public ICropVarietyRepository CropVarietyRepository =>
        _cropVarietyRepository ??= new CropVarietyRepository(_dbContext);

    public ISoilRepository SoilRepository =>
        _soilRepository ??= new SoilRepository(_dbContext);

    public IFieldCropHistoryRepository FieldCropHistoryRepository =>
        _fieldCropHistoryRepository ??= new FieldCropHistoryRepository(_dbContext);

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public TransactionScope BeginTransaction()
    {
        return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }
}
