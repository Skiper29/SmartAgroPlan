using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;

namespace SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

/// <summary>
///     Service for recording actual fertilizer applications
/// </summary>
public interface IFertilizerApplicationRecordService
{
    /// <summary>
    ///     Records an actual fertilizer application
    /// </summary>
    Task<int> RecordApplicationAsync(
        int fieldId,
        DateTime applicationDate,
        int applicationMethodId,
        Dictionary<int, double> productsUsed, // ProductId -> Quantity (kg/ha)
        int? applicationPlanId = null,
        string? notes = null,
        double? temperature = null,
        double? windSpeed = null,
        double? humidity = null);

    /// <summary>
    ///     Gets application history for a field
    /// </summary>
    Task<List<FertilizerApplicationRecord>> GetApplicationHistoryAsync(
        int fieldId,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    /// <summary>
    ///     Gets details of a specific application record
    /// </summary>
    Task<FertilizerApplicationRecord?> GetApplicationRecordDetailsAsync(int recordId);

    /// <summary>
    ///     Updates an application record
    /// </summary>
    Task UpdateApplicationRecordAsync(int recordId, FertilizerApplicationRecord updatedRecord);

    /// <summary>
    ///     Deletes an application record
    /// </summary>
    Task DeleteApplicationRecordAsync(int recordId);
}