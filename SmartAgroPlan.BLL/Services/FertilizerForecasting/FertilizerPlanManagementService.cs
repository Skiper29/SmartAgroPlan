using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;
using SmartAgroPlan.BLL.Models.FertilizerForecasting;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Applications;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.Services.FertilizerForecasting;

public class FertilizerPlanManagementService : IFertilizerPlanManagementService
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public FertilizerPlanManagementService(IRepositoryWrapper repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<int> SaveSeasonPlanAsync(SeasonFertilizerPlan plan, int fieldId)
    {
        // Get the fertilization plan for this crop
        var field = await _repository.FieldRepository.GetFirstOrDefaultAsync(
            f => f.Id == fieldId,
            f => f.Include(x => x.CurrentCrop)!);

        if (field?.CurrentCrop == null)
            throw new InvalidOperationException("Field or crop not found");

        var fertPlan = await _repository.FertilizationPlanRepository
            .GetFirstOrDefaultAsync(fp => fp.CropType == field.CurrentCrop.CropType,
                fp => fp.Include(p => p.Stages));

        if (fertPlan == null)
            throw new InvalidOperationException("No fertilization plan template found for this crop type");

        // Save each application as a plan
        foreach (var application in plan.Applications)
        {
            var planStage = fertPlan.Stages.FirstOrDefault(s =>
                Math.Abs((CalculateStageDay(field.CurrentCrop, s, plan.SowingDate) - plan.SowingDate).Days -
                         application.DaysAfterPlanting) < 5);

            var applicationPlan = new FertilizerApplicationPlan
            {
                FieldId = fieldId,
                FertilizationPlanId = fertPlan.Id,
                PlanStageId = planStage?.Id ?? fertPlan.Stages.First().Id,
                CreatedDate = DateTime.Now,
                PlannedApplicationDate = application.RecommendedDate,
                DaysAfterPlanting = application.DaysAfterPlanting,
                PlannedNitrogen = application.NutrientsToApply.Nitrogen,
                PlannedPhosphorus = application.NutrientsToApply.Phosphorus,
                PlannedPotassium = application.NutrientsToApply.Potassium,
                PlannedSulfur = application.NutrientsToApply.Sulfur,
                PlannedCalcium = application.NutrientsToApply.Calcium,
                PlannedMagnesium = application.NutrientsToApply.Magnesium,
                PlannedBoron = application.NutrientsToApply.Boron,
                PlannedZinc = application.NutrientsToApply.Zinc,
                PlannedManganese = application.NutrientsToApply.Manganese,
                PlannedCopper = application.NutrientsToApply.Copper,
                PlannedIron = application.NutrientsToApply.Iron,
                PlannedMolybdenum = application.NutrientsToApply.Molybdenum,
                IsCompleted = false
            };

            await _repository.FertilizerApplicationPlanRepository.CreateAsync(applicationPlan);
        }

        await _repository.SaveChangesAsync();
        plan.IsSaved = true;

        return fieldId;
    }

    public async Task<List<FertilizerApplication>> GetSavedApplicationPlansAsync(int fieldId,
        bool includeCompleted = false)
    {
        var plans = await _repository.FertilizerApplicationPlanRepository
            .GetAllAsync(p => p.FieldId == fieldId &&
                              (includeCompleted || !p.IsCompleted),
                p => p
                    .Include(pl => pl.PlanStage)
                    .ThenInclude(ps => ps!.ApplicationMethod)
                    .Include(pl => pl.Products)
                    .ThenInclude(pp => pp.FertilizerProduct)!);

        return plans.Select(p => new FertilizerApplication
        {
            Id = p.Id,
            RecommendedDate = p.PlannedApplicationDate,
            CropStage = p.PlanStage?.StageName ?? "N/A",
            DaysAfterPlanting = p.DaysAfterPlanting,
            NutrientsToApply = new NutrientRequirement
            {
                Nitrogen = p.PlannedNitrogen,
                Phosphorus = p.PlannedPhosphorus,
                Potassium = p.PlannedPotassium,
                Sulfur = p.PlannedSulfur,
                Calcium = p.PlannedCalcium,
                Magnesium = p.PlannedMagnesium,
                Boron = p.PlannedBoron,
                Zinc = p.PlannedZinc,
                Manganese = p.PlannedManganese,
                Copper = p.PlannedCopper,
                Iron = p.PlannedIron,
                Molybdenum = p.PlannedMolybdenum
            },
            Products = _mapper.Map<List<FertilizerProductDto>>(p.Products.Select(pr => pr.FertilizerProduct!).ToList()),
            ApplicationMethod = p.PlanStage?.ApplicationMethod?.Name ?? "N/A",
            Rationale = p.PlanStage?.Rationale ?? "",
            IsCompleted = p.IsCompleted,
            ActualApplicationDate = p.ActualApplicationDate
        }).ToList();
    }

    public async Task<List<FertilizerApplication>> GetUpcomingApplicationsAsync(int fieldId, int daysAhead = 14)
    {
        var startDate = DateTime.Now;
        var endDate = startDate.AddDays(daysAhead);

        var plans = await _repository.FertilizerApplicationPlanRepository
            .GetAllAsync(p => p.FieldId == fieldId &&
                              !p.IsCompleted &&
                              p.PlannedApplicationDate >= startDate &&
                              p.PlannedApplicationDate <= endDate,
                p => p
                    .Include(pl => pl.PlanStage)
                    .ThenInclude(ps => ps!.ApplicationMethod)
                    .Include(pl => pl.Products)
                    .ThenInclude(pp => pp.FertilizerProduct)!);

        return plans.Select(p => new FertilizerApplication
        {
            Id = p.Id,
            RecommendedDate = p.PlannedApplicationDate,
            CropStage = p.PlanStage?.StageName ?? "N/A",
            DaysAfterPlanting = p.DaysAfterPlanting,
            NutrientsToApply = new NutrientRequirement
            {
                Nitrogen = p.PlannedNitrogen,
                Phosphorus = p.PlannedPhosphorus,
                Potassium = p.PlannedPotassium,
                Sulfur = p.PlannedSulfur,
                Calcium = p.PlannedCalcium,
                Magnesium = p.PlannedMagnesium,
                Boron = p.PlannedBoron,
                Zinc = p.PlannedZinc,
                Manganese = p.PlannedManganese,
                Copper = p.PlannedCopper,
                Iron = p.PlannedIron,
                Molybdenum = p.PlannedMolybdenum
            },
            Products = _mapper.Map<List<FertilizerProductDto>>(p.Products.Select(pr => pr.FertilizerProduct!).ToList()),
            ApplicationMethod = p.PlanStage?.ApplicationMethod?.Name ?? "N/A",
            Rationale = p.PlanStage?.Rationale ?? "",
            IsCompleted = p.IsCompleted
        }).ToList();
    }

    public async Task UpdateApplicationPlanAsync(int applicationPlanId, FertilizerApplication updatedApplication)
    {
        var plan = await _repository.FertilizerApplicationPlanRepository
            .GetFirstOrDefaultAsync(p => p.Id == applicationPlanId);

        if (plan == null)
            throw new ArgumentException($"Application plan {applicationPlanId} not found");

        plan.PlannedApplicationDate = updatedApplication.RecommendedDate;
        plan.PlannedNitrogen = updatedApplication.NutrientsToApply.Nitrogen;
        plan.PlannedPhosphorus = updatedApplication.NutrientsToApply.Phosphorus;
        plan.PlannedPotassium = updatedApplication.NutrientsToApply.Potassium;
        plan.PlannedSulfur = updatedApplication.NutrientsToApply.Sulfur;
        plan.PlannedCalcium = updatedApplication.NutrientsToApply.Calcium;
        plan.PlannedMagnesium = updatedApplication.NutrientsToApply.Magnesium;
        plan.PlannedBoron = updatedApplication.NutrientsToApply.Boron;
        plan.PlannedZinc = updatedApplication.NutrientsToApply.Zinc;
        plan.PlannedManganese = updatedApplication.NutrientsToApply.Manganese;
        plan.PlannedCopper = updatedApplication.NutrientsToApply.Copper;
        plan.PlannedIron = updatedApplication.NutrientsToApply.Iron;
        plan.PlannedMolybdenum = updatedApplication.NutrientsToApply.Molybdenum;

        _repository.FertilizerApplicationPlanRepository.Update(plan);
        await _repository.SaveChangesAsync();
    }

    public async Task MarkApplicationCompletedAsync(int applicationPlanId, DateTime actualDate)
    {
        var plan = await _repository.FertilizerApplicationPlanRepository
            .GetFirstOrDefaultAsync(p => p.Id == applicationPlanId);

        if (plan == null)
            throw new ArgumentException($"Application plan {applicationPlanId} not found");

        plan.IsCompleted = true;
        plan.ActualApplicationDate = actualDate;

        _repository.FertilizerApplicationPlanRepository.Update(plan);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteApplicationPlanAsync(int applicationPlanId)
    {
        var plan = await _repository.FertilizerApplicationPlanRepository
            .GetFirstOrDefaultAsync(p => p.Id == applicationPlanId);

        if (plan == null)
            throw new ArgumentException($"Application plan {applicationPlanId} not found");

        _repository.FertilizerApplicationPlanRepository.Delete(plan);
        await _repository.SaveChangesAsync();
    }

    public async Task<NutrientApplicationSummary> GetApplicationSummaryAsync(int fieldId, DateTime fromDate,
        DateTime toDate)
    {
        var field = await _repository.FieldRepository.GetFirstOrDefaultAsync(f => f.Id == fieldId);

        var plans = await _repository.FertilizerApplicationPlanRepository
            .GetAllAsync(p => p.FieldId == fieldId &&
                              p.PlannedApplicationDate >= fromDate &&
                              p.PlannedApplicationDate <= toDate,
                p => p.Include(pl => pl.PlanStage)!);

        var plansList = plans.ToList();

        var records = await _repository.FertilizerApplicationRecordRepository
            .GetAllAsync(r => r.ApplicationPlan != null &&
                              r.ApplicationPlan.FieldId == fieldId &&
                              r.ApplicationDate >= fromDate &&
                              r.ApplicationDate <= toDate);

        var totalApplied = new NutrientRequirement();
        foreach (var record in records)
        {
            totalApplied.Nitrogen += record.AppliedNitrogen;
            totalApplied.Phosphorus += record.AppliedPhosphorus;
            totalApplied.Potassium += record.AppliedPotassium;
            totalApplied.Sulfur += record.AppliedSulfur;
            totalApplied.Calcium += record.AppliedCalcium;
            totalApplied.Magnesium += record.AppliedMagnesium;
            totalApplied.Boron += record.AppliedBoron;
            totalApplied.Zinc += record.AppliedZinc;
            totalApplied.Manganese += record.AppliedManganese;
            totalApplied.Copper += record.AppliedCopper;
            totalApplied.Iron += record.AppliedIron;
            totalApplied.Molybdenum += record.AppliedMolybdenum;
        }

        var plannedToApply = new NutrientRequirement();

        var applications = new List<ApplicationSummaryItem>();
        var completedCount = 0;
        var pendingCount = 0;

        foreach (var plan in plansList)
        {
            if (!plan.IsCompleted)
            {
                pendingCount++;
                plannedToApply.Nitrogen += plan.PlannedNitrogen;
                plannedToApply.Phosphorus += plan.PlannedPhosphorus;
                plannedToApply.Potassium += plan.PlannedPotassium;
                plannedToApply.Sulfur += plan.PlannedSulfur;
                plannedToApply.Calcium += plan.PlannedCalcium;
                plannedToApply.Magnesium += plan.PlannedMagnesium;
                plannedToApply.Boron += plan.PlannedBoron;
                plannedToApply.Zinc += plan.PlannedZinc;
                plannedToApply.Manganese += plan.PlannedManganese;
                plannedToApply.Copper += plan.PlannedCopper;
                plannedToApply.Iron += plan.PlannedIron;
                plannedToApply.Molybdenum += plan.PlannedMolybdenum;
            }
            else
            {
                completedCount++;
            }

            applications.Add(new ApplicationSummaryItem
            {
                Id = plan.Id,
                Date = plan.PlannedApplicationDate,
                IsCompleted = plan.IsCompleted,
                Stage = plan.PlanStage?.StageName ?? "N/A",
                Nutrients = new NutrientRequirement
                {
                    Nitrogen = plan.PlannedNitrogen,
                    Phosphorus = plan.PlannedPhosphorus,
                    Potassium = plan.PlannedPotassium,
                    Sulfur = plan.PlannedSulfur,
                    Calcium = plan.PlannedCalcium,
                    Magnesium = plan.PlannedMagnesium,
                    Boron = plan.PlannedBoron,
                    Zinc = plan.PlannedZinc,
                    Manganese = plan.PlannedManganese,
                    Copper = plan.PlannedCopper,
                    Iron = plan.PlannedIron,
                    Molybdenum = plan.PlannedMolybdenum
                }
            });
        }

        return new NutrientApplicationSummary
        {
            FieldId = fieldId,
            FieldName = field?.Name,
            FromDate = fromDate,
            ToDate = toDate,
            TotalApplied = totalApplied,
            PlannedToApply = plannedToApply,
            CompletedApplications = completedCount,
            PendingApplications = pendingCount,
            Applications = applications
        };
    }

    private DateTime CalculateStageDay(CropVariety crop, PlanStage stage, DateTime sowingDate)
    {
        return stage.GrowthStage switch
        {
            GrowthStage.PreSowing => sowingDate.AddDays(stage.TimingFactor),
            GrowthStage.Initial => sowingDate.AddDays(crop.LIni * stage.TimingFactor),
            GrowthStage.Development => sowingDate.AddDays(crop.LIni + crop.LDev * stage.TimingFactor),
            GrowthStage.MidSeason => sowingDate.AddDays(crop.LIni + crop.LDev + crop.LMid * stage.TimingFactor),
            GrowthStage.LateSeason => sowingDate.AddDays(crop.LIni + crop.LDev + crop.LMid +
                                                         crop.LLate * stage.TimingFactor),
            _ => sowingDate
        };
    }
}
