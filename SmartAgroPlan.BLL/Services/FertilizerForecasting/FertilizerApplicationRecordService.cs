using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.Services.FertilizerForecasting;

public class FertilizerApplicationRecordService : IFertilizerApplicationRecordService
{
    private readonly IRepositoryWrapper _repository;

    public FertilizerApplicationRecordService(IRepositoryWrapper repository)
    {
        _repository = repository;
    }

    public async Task<int> RecordApplicationAsync(
        int fieldId,
        DateTime applicationDate,
        int applicationMethodId,
        Dictionary<int, double> productsUsed,
        int? applicationPlanId = null,
        string? notes = null,
        double? temperature = null,
        double? windSpeed = null,
        double? humidity = null)
    {
        // Calculate total nutrients from products
        var totalNutrients = await CalculateNutrientsFromProducts(productsUsed);

        var record = new FertilizerApplicationRecord
        {
            FieldId = fieldId,
            ApplicationPlanId = applicationPlanId,
            ApplicationDate = applicationDate,
            RecordedDate = DateTime.Now,
            AppliedNitrogen = totalNutrients.Nitrogen,
            AppliedPhosphorus = totalNutrients.Phosphorus,
            AppliedPotassium = totalNutrients.Potassium,
            AppliedSulfur = totalNutrients.Sulfur,
            AppliedCalcium = totalNutrients.Calcium,
            AppliedMagnesium = totalNutrients.Magnesium,
            AppliedBoron = totalNutrients.Boron,
            AppliedZinc = totalNutrients.Zinc,
            AppliedManganese = totalNutrients.Manganese,
            AppliedCopper = totalNutrients.Copper,
            AppliedIron = totalNutrients.Iron,
            AppliedMolybdenum = totalNutrients.Molybdenum,
            ApplicationMethodId = applicationMethodId,
            Notes = notes,
            TemperatureC = temperature,
            WindSpeedKmh = windSpeed,
            Humidity = humidity
        };

        var createdRecord = await _repository.FertilizerApplicationRecordRepository.CreateAsync(record);
        await _repository.SaveChangesAsync();

        // Create product records
        var productRecordList = new List<FertilizerApplicationRecordProduct>();
        foreach (var (productId, quantity) in productsUsed)
            productRecordList.Add(new FertilizerApplicationRecordProduct
            {
                ApplicationRecordId = createdRecord.Id,
                FertilizerProductId = productId,
                QuantityUsedKg = quantity
            });
        await _repository.FertilizerApplicationRecordProductRepository
            .CreateRangeAsync(productRecordList);
        await _repository.SaveChangesAsync();

        // If this completes a plan, mark it as completed
        if (applicationPlanId.HasValue)
        {
            var plan = await _repository.FertilizerApplicationPlanRepository
                .GetFirstOrDefaultAsync(p => p.Id == applicationPlanId.Value);

            if (plan != null)
            {
                plan.IsCompleted = true;
                plan.ActualApplicationDate = applicationDate;
                _repository.FertilizerApplicationPlanRepository.Update(plan);
                await _repository.SaveChangesAsync();
            }
        }

        return createdRecord.Id;
    }

    public async Task<List<FertilizerApplicationRecord>> GetApplicationHistoryAsync(
        int fieldId,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _repository.FertilizerApplicationRecordRepository
            .FindAll(r => r.FieldId == fieldId);

        if (fromDate.HasValue)
            query = query.Where(r => r.ApplicationDate >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(r => r.ApplicationDate <= toDate.Value);

        return await query
            .OrderByDescending(r => r.ApplicationDate)
            .ToListAsync();
    }

    public async Task<FertilizerApplicationRecord?> GetApplicationRecordDetailsAsync(int recordId)
    {
        return await _repository.FertilizerApplicationRecordRepository
            .GetFirstOrDefaultAsync(r => r.Id == recordId,
                q => q.Include(r => r.ApplicationMethod)
                    .Include(r => r.ApplicationPlan)
                    .ThenInclude(p => p!.PlanStage)!);
    }

    public async Task UpdateApplicationRecordAsync(int recordId, FertilizerApplicationRecord updatedRecord)
    {
        var record = await _repository.FertilizerApplicationRecordRepository
            .GetFirstOrDefaultAsync(r => r.Id == recordId);

        if (record == null)
            throw new ArgumentException($"Запис про внесення з ID {recordId} не знайдено");

        record.ApplicationDate = updatedRecord.ApplicationDate;
        record.AppliedNitrogen = updatedRecord.AppliedNitrogen;
        record.AppliedPhosphorus = updatedRecord.AppliedPhosphorus;
        record.AppliedPotassium = updatedRecord.AppliedPotassium;
        record.AppliedSulfur = updatedRecord.AppliedSulfur;
        record.AppliedCalcium = updatedRecord.AppliedCalcium;
        record.AppliedMagnesium = updatedRecord.AppliedMagnesium;
        record.AppliedBoron = updatedRecord.AppliedBoron;
        record.AppliedZinc = updatedRecord.AppliedZinc;
        record.AppliedManganese = updatedRecord.AppliedManganese;
        record.AppliedCopper = updatedRecord.AppliedCopper;
        record.AppliedIron = updatedRecord.AppliedIron;
        record.AppliedMolybdenum = updatedRecord.AppliedMolybdenum;
        record.Notes = updatedRecord.Notes;
        record.TemperatureC = updatedRecord.TemperatureC;
        record.WindSpeedKmh = updatedRecord.WindSpeedKmh;
        record.Humidity = updatedRecord.Humidity;

        _repository.FertilizerApplicationRecordRepository.Update(record);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteApplicationRecordAsync(int recordId)
    {
        var record = await _repository.FertilizerApplicationRecordRepository
            .GetFirstOrDefaultAsync(r => r.Id == recordId);

        if (record == null)
            throw new ArgumentException($"Запис про внесення з ID {recordId} не знайдено");

        _repository.FertilizerApplicationRecordRepository.Delete(record);
        await _repository.SaveChangesAsync();
    }

    private async Task<(double Nitrogen, double Phosphorus, double Potassium, double Sulfur, double Calcium, double
            Magnesium, double Boron, double Zinc, double Manganese, double Copper, double Iron, double Molybdenum
            )>
        CalculateNutrientsFromProducts(Dictionary<int, double> productsUsed)
    {
        double n = 0, p = 0, k = 0, s = 0, ca = 0, mg = 0, b = 0, zn = 0, mn = 0, cu = 0, fe = 0, mo = 0;

        var productIds = productsUsed.Keys;

        var allProductsList = await _repository.FertilizerProductRepository
            .GetAllAsync(pr => productIds.Contains(pr.Id));

        var productDictionary = allProductsList.ToDictionary(prod => prod.Id);

        foreach (var (productId, quantityKgPerHa) in productsUsed)
            if (productDictionary.TryGetValue(productId, out var product))
            {
                n += quantityKgPerHa * (product.NitrogenContent / 100.0);
                p += quantityKgPerHa * (product.PhosphorusContent / 100.0);
                k += quantityKgPerHa * (product.PotassiumContent / 100.0);
                s += quantityKgPerHa * ((product.SulfurContent ?? 0) / 100.0);
                ca += quantityKgPerHa * ((product.CalciumContent ?? 0) / 100.0);
                mg += quantityKgPerHa * ((product.MagnesiumContent ?? 0) / 100.0);
                b += quantityKgPerHa * ((product.BoronContent ?? 0) / 100.0);
                zn += quantityKgPerHa * ((product.ZincContent ?? 0) / 100.0);
                mn += quantityKgPerHa * ((product.ManganeseContent ?? 0) / 100.0);
                cu += quantityKgPerHa * ((product.CopperContent ?? 0) / 100.0);
                fe += quantityKgPerHa * ((product.IronContent ?? 0) / 100.0);
                mo += quantityKgPerHa * ((product.MolybdenumContent ?? 0) / 100.0);
            }

        return (n, p, k, s, ca, mg, b, zn, mn, cu, fe, mo);
    }
}
