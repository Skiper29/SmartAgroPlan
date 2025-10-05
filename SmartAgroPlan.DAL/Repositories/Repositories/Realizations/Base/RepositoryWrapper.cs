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
    private ICropCoefficientDefinitionRepository? _cropCoefficientDefinitionRepository;
    private ICropVarietyRepository? _cropVarietyRepository;
    private IFieldConditionRepository? _fieldConditionRepository;
    private IFieldCropHistoryRepository? _fieldCropHistoryRepository;
    private IFieldRepository? _fieldRepository;
    private ISoilRepository? _soilRepository;

    public RepositoryWrapper(SmartAgroPlanDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IFieldConditionRepository FieldConditionRepository =>
        _fieldConditionRepository ??= new FieldConditionRepository(_dbContext);

    public IFieldRepository FieldRepository =>
        _fieldRepository ??= new FieldRepository(_dbContext);

    public ICropVarietyRepository CropVarietyRepository =>
        _cropVarietyRepository ??= new CropVarietyRepository(_dbContext);

    public ISoilRepository SoilRepository =>
        _soilRepository ??= new SoilRepository(_dbContext);

    public IFieldCropHistoryRepository FieldCropHistoryRepository =>
        _fieldCropHistoryRepository ??= new FieldCropHistoryRepository(_dbContext);

    public ICropCoefficientDefinitionRepository CropCoefficientDefinitionRepository =>
        _cropCoefficientDefinitionRepository ??= new CropCoefficientDefinitionRepository(_dbContext);

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
