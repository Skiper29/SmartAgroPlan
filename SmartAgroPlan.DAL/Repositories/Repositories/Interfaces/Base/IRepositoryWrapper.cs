using System.Transactions;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Fields;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    IFieldRepository FieldRepository { get; }
    ICropVarietyRepository CropVarietyRepository { get; }
    ISoilRepository SoilRepository { get; }
    IFieldCropHistoryRepository FieldCropHistoryRepository { get; }
    void SaveChanges();
    Task<int> SaveChangesAsync();
    TransactionScope BeginTransaction();
}
