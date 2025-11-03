using System.Transactions;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.FertilizerForecasting;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Fields;

namespace SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    IFieldRepository FieldRepository { get; }
    ICropVarietyRepository CropVarietyRepository { get; }
    ISoilRepository SoilRepository { get; }
    IFieldCropHistoryRepository FieldCropHistoryRepository { get; }
    IFieldConditionRepository FieldConditionRepository { get; }
    ICropCoefficientDefinitionRepository CropCoefficientDefinitionRepository { get; }

    // Fertilizer Forecasting Repositories
    IFertilizationPlanRepository FertilizationPlanRepository { get; }
    IFertilizerApplicationPlanRepository FertilizerApplicationPlanRepository { get; }
    IFertilizerApplicationRecordRepository FertilizerApplicationRecordRepository { get; }
    IFertilizerProductRepository FertilizerProductRepository { get; }
    IApplicationMethodRepository ApplicationMethodRepository { get; }
    IFertilizerApplicationRecordProductRepository FertilizerApplicationRecordProductRepository { get; }

    void SaveChanges();
    Task<int> SaveChangesAsync();
    TransactionScope BeginTransaction();
}