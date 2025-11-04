using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;
using SmartAgroPlan.BLL.Models.FertilizerForecasting;
using SmartAgroPlan.BLL.Models.FertilizerForecasting.Nutrients;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Applications;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.Services.FertilizerForecasting;

public class FertilizerCalculationService : IFertilizerCalculationService
{
    // Crop-specific nutrient uptake coefficients (kg/ton of yield)
    private readonly Dictionary<CropType, NutrientUptakeCoefficients> _cropCoefficients = new()
    {
        [CropType.Wheat] = new NutrientUptakeCoefficients
        {
            N = 20.0, P = 4.0, K = 5.0, S = 2.0, Ca = 1.5, Mg = 1.2, B = 0.01, Zn = 0.02, Mn = 0.03, Cu = 0.005,
            Fe = 0.05, Mo = 0.001
        },
        [CropType.Corn] = new NutrientUptakeCoefficients
        {
            N = 22.0, P = 4.5, K = 6.5, S = 2.5, Ca = 2.0, Mg = 1.5, B = 0.015, Zn = 0.025, Mn = 0.04, Cu = 0.007,
            Fe = 0.06, Mo = 0.001
        },
        [CropType.Barley] = new NutrientUptakeCoefficients
        {
            N = 18.0, P = 3.5, K = 4.5, S = 1.8, Ca = 1.3, Mg = 1.0, B = 0.008, Zn = 0.015, Mn = 0.025, Cu = 0.004,
            Fe = 0.04, Mo = 0.001
        },
        [CropType.Sunflower] = new NutrientUptakeCoefficients
        {
            N = 25.0, P = 6.0, K = 12.0, S = 3.0, Ca = 3.5, Mg = 2.0, B = 0.02, Zn = 0.03, Mn = 0.05, Cu = 0.01,
            Fe = 0.08, Mo = 0.001
        },
        [CropType.Soy] = new NutrientUptakeCoefficients
        {
            N = 15.0, P = 5.0, K = 15.0, S = 2.2, Ca = 2.5, Mg = 1.8, B = 0.012, Zn = 0.02, Mn = 0.035, Cu = 0.006,
            Fe = 0.055, Mo = 0.002
        },
        [CropType.Rapeseed] = new NutrientUptakeCoefficients
        {
            N = 30.0, P = 7.0, K = 10.0, S = 5.0, Ca = 4.0, Mg = 2.5, B = 0.025, Zn = 0.02, Mn = 0.04, Cu = 0.008,
            Fe = 0.07, Mo = 0.001
        },
        [CropType.Potato] = new NutrientUptakeCoefficients
        {
            N = 4.5, P = 0.8, K = 6.0, S = 0.5, Ca = 0.3, Mg = 0.4, B = 0.005, Zn = 0.01, Mn = 0.015, Cu = 0.003,
            Fe = 0.02, Mo = 0.0005
        },
        [CropType.SugarBeet] = new NutrientUptakeCoefficients
        {
            N = 4.0, P = 1.0, K = 5.0, S = 0.6, Ca = 1.0, Mg = 0.5, B = 0.008, Zn = 0.012, Mn = 0.02, Cu = 0.004,
            Fe = 0.03, Mo = 0.0005
        },
        [CropType.Tomato] = new NutrientUptakeCoefficients
        {
            N = 3.0, P = 0.6, K = 4.5, S = 0.4, Ca = 1.2, Mg = 0.3, B = 0.006, Zn = 0.015, Mn = 0.018, Cu = 0.003,
            Fe = 0.025, Mo = 0.0005
        },
        [CropType.Another] = new NutrientUptakeCoefficients
        {
            N = 20.0, P = 5.0, K = 7.0, S = 2.0, Ca = 2.0, Mg = 1.5, B = 0.01, Zn = 0.02, Mn = 0.03, Cu = 0.005,
            Fe = 0.05, Mo = 0.001
        }
    };

    private readonly IMapper _mapper;

    private readonly IRepositoryWrapper _repository;

    public FertilizerCalculationService(IRepositoryWrapper repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SeasonFertilizerPlan> CalculateSeasonPlanAsync(
        int fieldId,
        double targetYield,
        DateTime? customSowingDate = null)
    {
        // Load field with all necessary data
        var field = await _repository.FieldRepository.GetFirstOrDefaultAsync(
            f => f.Id == fieldId,
            f => f.Include(e => e.CurrentCrop)
                .Include(e => e.Soil)
                .Include(e => e.Conditions!.OrderByDescending(c => c.RecordedAt).Take(1))
        );

        if (field == null)
            throw new ArgumentException($"Field with ID {fieldId} not found");

        if (field.CurrentCrop == null)
            throw new InvalidOperationException($"Field {fieldId} has no current crop assigned");

        var sowingDate = customSowingDate ??
                         field.SowingDate ?? throw new InvalidOperationException("Sowing date not specified");
        var latestCondition = field.Conditions?.FirstOrDefault();

        // Calculate field area
        var fieldAreaHa = await _repository.FieldRepository.FindAll(f => f.Id == fieldId)
            .Select(f => f.Boundary!.Area / 10000.0)
            .FirstOrDefaultAsync();

        // Get fertilization plan template for this crop type
        var fertPlan = await _repository.FertilizationPlanRepository.GetFirstOrDefaultAsync(
            p => p.CropType == field.CurrentCrop.CropType,
            p => p.Include(plan => plan.Stages)
                .ThenInclude(stage => stage.ApplicationMethod)!
        );

        // Calculate nutrient requirements
        var totalRequirement = CalculateTotalNutrientRequirement(field.CurrentCrop, targetYield, fieldAreaHa);
        var soilSupply = CalculateSoilNutrientSupply(field.Soil!, latestCondition, field.CurrentCrop);

        var requiredFromFertilizer = SubtractNutrients(totalRequirement, soilSupply);

        // Get already applied nutrients
        var alreadyApplied = await CalculateAlreadyAppliedNutrientsAsync(fieldId, sowingDate);
        var remainingToApply = SubtractNutrients(requiredFromFertilizer, alreadyApplied);

        // Generate applications based on plan stages
        var applications = await GenerateApplicationScheduleAsync(
            field.CurrentCrop,
            fertPlan,
            remainingToApply,
            sowingDate,
            fieldAreaHa);

        return new SeasonFertilizerPlan
        {
            FieldId = fieldId,
            FieldName = field.Name,
            CropName = field.CurrentCrop.Name,
            SowingDate = sowingDate,
            ExpectedHarvestDate = sowingDate.AddDays(field.CurrentCrop.GrowingDuration),
            PlanGeneratedDate = DateTime.Now,
            TotalSeasonRequirement = totalRequirement,
            SoilSupply = soilSupply,
            RequiredFromFertilizer = requiredFromFertilizer,
            AlreadyApplied = alreadyApplied,
            RemainingToApply = remainingToApply,
            Applications = applications,
            FieldAreaHa = fieldAreaHa,
            ExpectedYield = targetYield,
            IsSaved = false
        };
    }

    public async Task<CurrentRecommendation> GetCurrentRecommendationAsync(int fieldId)
    {
        var field = await _repository.FieldRepository.GetFirstOrDefaultAsync(
            f => f.Id == fieldId,
            f => f.Include(e => e.CurrentCrop)
                .Include(e => e.Soil)
                .Include(e => e.Conditions!.OrderByDescending(c => c.RecordedAt).Take(1))
        );

        if (field == null)
            throw new ArgumentException($"Field with ID {fieldId} not found");

        if (field.SowingDate == null)
            throw new InvalidOperationException("Field has no sowing date");

        var now = DateTime.Now;
        var daysAfterPlanting = (now - field.SowingDate.Value).Days;
        var currentStage = DetermineGrowthStage(field.CurrentCrop!, daysAfterPlanting);
        var daysToHarvest = field.CurrentCrop!.GrowingDuration - daysAfterPlanting;

        // Get upcoming planned applications
        var upcomingApplications = await _repository.FertilizerApplicationPlanRepository
            .GetAllAsync(a => a.FieldId == fieldId
                              && !a.IsCompleted
                              && a.PlannedApplicationDate >= now && a.PlannedApplicationDate <= now.AddDays(30),
                q => q.Include(a => a.PlanStage)
                    .ThenInclude(s => s!.ApplicationMethod)
                    .Include(a => a.Products)
                    .ThenInclude(p => p.FertilizerProduct)!
            );

        var upcomingFertilizerApplicationPlans =
            upcomingApplications as FertilizerApplicationPlan[] ?? upcomingApplications.ToArray();
        var shouldApplyNow = upcomingFertilizerApplicationPlans.Any(a =>
            Math.Abs((a.PlannedApplicationDate - now).TotalDays) <= 3);

        var recommendation = upcomingFertilizerApplicationPlans.FirstOrDefault(a =>
            Math.Abs((a.PlannedApplicationDate - now).TotalDays) <= 3);

        NutrientRequirement recommendedNutrients;
        List<FertilizerProductDto> products = new();
        var applicationMethod = "Не визначено";
        string priority;
        string reasoning;

        if (recommendation != null)
        {
            recommendedNutrients = new NutrientRequirement
            {
                Nitrogen = recommendation.PlannedNitrogen,
                Phosphorus = recommendation.PlannedPhosphorus,
                Potassium = recommendation.PlannedPotassium,
                Sulfur = recommendation.PlannedSulfur,
                Calcium = recommendation.PlannedCalcium,
                Magnesium = recommendation.PlannedMagnesium,
                Boron = recommendation.PlannedBoron,
                Zinc = recommendation.PlannedZinc,
                Manganese = recommendation.PlannedManganese,
                Copper = recommendation.PlannedCopper,
                Iron = recommendation.PlannedIron,
                Molybdenum = recommendation.PlannedMolybdenum
            };

            products = _mapper.Map<List<FertilizerProductDto>>(recommendation.Products.Select(p => p.FertilizerProduct!)
                .ToList());
            applicationMethod = recommendation.PlanStage?.ApplicationMethod?.Name ?? "Не визначено";
            priority = DeterminePriority(daysAfterPlanting, currentStage, field.CurrentCrop);
            reasoning = recommendation.PlanStage?.Rationale ?? "Згідно з планом підживлення";
        }
        else
        {
            recommendedNutrients = new NutrientRequirement();
            priority = "Low";
            reasoning = "Немає запланованих внесень на найближчий час";
        }

        var warnings = GenerateWarnings(field, currentStage, daysToHarvest);

        return new CurrentRecommendation
        {
            FieldId = fieldId,
            FieldName = field.Name,
            Date = now,
            CurrentStage = currentStage,
            DaysAfterPlanting = daysAfterPlanting,
            DaysToHarvest = daysToHarvest,
            ShouldApplyNow = shouldApplyNow,
            RecommendedNutrients = recommendedNutrients,
            Products = products,
            ApplicationMethod = applicationMethod,
            Priority = priority,
            Reasoning = reasoning,
            Warnings = warnings,
            NextRecommendedDate = upcomingFertilizerApplicationPlans.FirstOrDefault()?.PlannedApplicationDate
        };
    }

    public async Task<List<FertilizerApplication>> GetApplicationsForDateRangeAsync(
        int fieldId,
        DateTime startDate,
        DateTime endDate)
    {
        var applications = await _repository.FertilizerApplicationPlanRepository
            .GetAllAsync(a => a.FieldId == fieldId
                              && a.PlannedApplicationDate >= startDate
                              && a.PlannedApplicationDate <= endDate,
                q => q.Include(a => a.PlanStage)
                    .ThenInclude(s => s!.ApplicationMethod)
                    .Include(a => a.Products)
                    .ThenInclude(p => p.FertilizerProduct)!
            );

        return applications.Select(a => new FertilizerApplication
        {
            Id = a.Id,
            RecommendedDate = a.PlannedApplicationDate,
            CropStage = a.PlanStage?.StageName ?? "N/A",
            DaysAfterPlanting = a.DaysAfterPlanting,
            NutrientsToApply = new NutrientRequirement
            {
                Nitrogen = a.PlannedNitrogen,
                Phosphorus = a.PlannedPhosphorus,
                Potassium = a.PlannedPotassium,
                Sulfur = a.PlannedSulfur,
                Calcium = a.PlannedCalcium,
                Magnesium = a.PlannedMagnesium,
                Boron = a.PlannedBoron,
                Zinc = a.PlannedZinc,
                Manganese = a.PlannedManganese,
                Copper = a.PlannedCopper,
                Iron = a.PlannedIron,
                Molybdenum = a.PlannedMolybdenum
            },
            Products = _mapper.Map<List<FertilizerProductDto>>(a.Products.Select(p => p.FertilizerProduct!).ToList()),
            ApplicationMethod = a.PlanStage?.ApplicationMethod?.Name ?? "N/A",
            Rationale = a.PlanStage?.Rationale ?? "",
            IsCompleted = a.IsCompleted,
            ActualApplicationDate = a.ActualApplicationDate
        }).ToList();
    }

    public async Task<NutrientDeficitAnalysis> AnalyzeNutrientDeficitAsync(int fieldId)
    {
        var field = await _repository.FieldRepository.GetFirstOrDefaultAsync(
            f => f.Id == fieldId,
            f => f.Include(e => e.CurrentCrop)
                .Include(e => e.Soil)
                .Include(e => e.Conditions!.OrderByDescending(c => c.RecordedAt).Take(1))
        );

        if (field == null)
            throw new ArgumentException($"Поле з ID {fieldId} не знайдено");

        var latestCondition = field.Conditions?.FirstOrDefault();
        if (latestCondition == null)
            throw new InvalidOperationException("Немає даних про стан ґрунту");

        var targetYield = field.CurrentCrop!.HarvestYield;
        var fieldAreaHa = await _repository.FieldRepository.FindAll(f => f.Id == fieldId)
            .Select(f => f.Boundary!.Area / 10000.0)
            .FirstOrDefaultAsync();

        var totalRequired = CalculateTotalNutrientRequirement(field.CurrentCrop, targetYield, fieldAreaHa);
        var soilSupply = CalculateSoilNutrientSupply(field.Soil!, latestCondition, field.CurrentCrop);
        var deficit = SubtractNutrients(totalRequired, soilSupply);

        var deficits = new List<NutrientDeficit>();

        if (deficit.Nitrogen > 0)
            deficits.Add(new NutrientDeficit
            {
                NutrientName = "Азот (N)",
                DeficitAmount = deficit.Nitrogen,
                Urgency = "High",
                Symptoms = "Пожовтіння листя, уповільнений ріст"
            });

        if (deficit.Phosphorus > 0)
            deficits.Add(new NutrientDeficit
            {
                NutrientName = "Фосфор (P₂O₅)",
                DeficitAmount = deficit.Phosphorus,
                Urgency = "Medium",
                Symptoms = "Фіолетове/темно-зелене забарвлення листя, слабкий розвиток кореневої системи"
            });

        if (deficit.Potassium > 0)
            deficits.Add(new NutrientDeficit
            {
                NutrientName = "Калій (K₂O)",
                DeficitAmount = deficit.Potassium,
                Urgency = "Medium",
                Symptoms = "Краї листя жовтіють і сохнуть, слабка стійкість до хвороб"
            });

        return new NutrientDeficitAnalysis
        {
            FieldId = fieldId,
            FieldName = field.Name!,
            AnalysisDate = DateTime.Now,
            Deficits = deficits,
            OverallStatus = deficits.Any(d => d.Urgency == "High") ? "Critical" : "Moderate",
            Recommendations = GenerateDeficitRecommendations(deficits)
        };
    }

    public async Task<NutrientBalance> GetNutrientBalanceAsync(int fieldId)
    {
        var field = await _repository.FieldRepository.GetFirstOrDefaultAsync(
            f => f.Id == fieldId,
            f => f.Include(e => e.CurrentCrop)
                .Include(e => e.Soil)
                .Include(e => e.Conditions!.OrderByDescending(c => c.RecordedAt).Take(1))
        );

        if (field == null)
            throw new ArgumentException($"Field with ID {fieldId} not found");

        var sowingDate = field.SowingDate ?? DateTime.Now.AddDays(-30);
        var daysAfterPlanting = (DateTime.Now - sowingDate).Days;
        var daysToHarvest = field.CurrentCrop!.GrowingDuration - daysAfterPlanting;

        var fieldAreaHa = await _repository.FieldRepository.FindAll(f => f.Id == fieldId)
            .Select(f => f.Boundary!.Area / 10000.0)
            .FirstOrDefaultAsync();

        var targetYield = field.CurrentCrop.HarvestYield;
        var totalRequired = CalculateTotalNutrientRequirement(field.CurrentCrop, targetYield, fieldAreaHa);
        var soilSupply =
            CalculateSoilNutrientSupply(field.Soil!, field.Conditions?.FirstOrDefault(), field.CurrentCrop);
        var alreadyApplied = await CalculateAlreadyAppliedNutrientsAsync(fieldId, sowingDate);

        var deficit = SubtractNutrients(SubtractNutrients(totalRequired, soilSupply), alreadyApplied);
        var surplus = new NutrientRequirement(); // Calculate if over-applied

        var overallStatus = DetermineOverallNutrientStatus(deficit);
        var recommendations = GenerateBalanceRecommendations(deficit, daysToHarvest);
        var warnings = GenerateBalanceWarnings(deficit, surplus);

        return new NutrientBalance
        {
            FieldId = fieldId,
            FieldName = field.Name,
            CropName = field.CurrentCrop.Name,
            AnalysisDate = DateTime.Now,
            DaysAfterPlanting = daysAfterPlanting,
            DaysToHarvest = daysToHarvest,
            RequiredForTargetYield = totalRequired,
            AvailableInSoil = soilSupply,
            AlreadyApplied = alreadyApplied,
            Deficit = deficit,
            Surplus = surplus,
            OverallStatus = overallStatus,
            Recommendations = recommendations,
            Warnings = warnings
        };
    }

    public NutrientRequirement CalculateTotalNutrientRequirement(
        CropVariety crop,
        double targetYield,
        double fieldAreaHa)
    {
        var coefficients = _cropCoefficients.GetValueOrDefault(crop.CropType, _cropCoefficients[CropType.Another]);

        return new NutrientRequirement
        {
            Nitrogen = coefficients.N * targetYield,
            Phosphorus = coefficients.P * targetYield * 2.29, // Convert P to P2O5
            Potassium = coefficients.K * targetYield * 1.2, // Convert K to K2O
            Sulfur = coefficients.S * targetYield,
            Calcium = coefficients.Ca * targetYield,
            Magnesium = coefficients.Mg * targetYield,
            Boron = coefficients.B * targetYield,
            Zinc = coefficients.Zn * targetYield,
            Manganese = coefficients.Mn * targetYield,
            Copper = coefficients.Cu * targetYield,
            Iron = coefficients.Fe * targetYield,
            Molybdenum = coefficients.Mo * targetYield
        };
    }

    public NutrientRequirement CalculateSoilNutrientSupply(
        Soil soil,
        FieldCondition? latestCondition,
        CropVariety crop)
    {
        var nSupply = latestCondition?.Nitrogen ?? 0;
        var pSupply = latestCondition?.Phosphorus ?? 0;
        var kSupply = latestCondition?.Potassium ?? 0;
        var sSupply = latestCondition?.Sulfur ?? 0;
        var caSupply = latestCondition?.Calcium ?? 0;
        var mgSupply = latestCondition?.Magnesium ?? 0;

        // Add mineralization contribution (organic matter releases nutrients)
        var mineralizationRate = soil.OrganicMatter * 20.0; // kg N/ha per season
        nSupply += mineralizationRate * (crop.GrowingDuration / 120.0);

        // Adjust for soil type efficiency
        var soilEfficiency = GetSoilEfficiencyFactor(soil.Type);

        return new NutrientRequirement
        {
            Nitrogen = nSupply * soilEfficiency,
            Phosphorus = pSupply * soilEfficiency,
            Potassium = kSupply * soilEfficiency,
            Sulfur = sSupply > 0 ? sSupply * soilEfficiency : 10 * soilEfficiency,
            Calcium = caSupply > 0 ? caSupply * soilEfficiency : 50 * soilEfficiency,
            Magnesium = mgSupply > 0 ? mgSupply * soilEfficiency : 30 * soilEfficiency,
            Boron = 0.5 * soilEfficiency,
            Zinc = 1.0 * soilEfficiency,
            Manganese = 2.0 * soilEfficiency,
            Copper = 0.3 * soilEfficiency,
            Iron = 3.0 * soilEfficiency,
            Molybdenum = 0.1 * soilEfficiency
        };
    }

    public async Task<ProductRecommendation> OptimizeProductSelectionAsync(
        NutrientRequirement targetNutrients,
        double fieldAreaHa,
        string optimizationStrategy = "Balanced")
    {
        var availableProducts = await _repository.FertilizerProductRepository.GetAllAsync();

        var recommendedProducts = new List<RecommendedProduct>();
        var actualNutrients = new NutrientRequirement();

        // Simple product selection (can be enhanced with optimization algorithms)
        if (targetNutrients.Nitrogen > 0)
        {
            var urea = availableProducts.FirstOrDefault(p => p.Name!.Contains("Urea") || p.NitrogenContent >= 45);
            if (urea != null)
            {
                var quantityPerHa = targetNutrients.Nitrogen / (urea.NitrogenContent / 100.0);
                recommendedProducts.Add(new RecommendedProduct
                {
                    ProductId = urea.Id,
                    ProductName = urea.Name!,
                    QuantityKgPerHa = quantityPerHa,
                    TotalQuantityKg = quantityPerHa * fieldAreaHa,
                    NutrientsProvided = new NutrientRequirement { Nitrogen = targetNutrients.Nitrogen },
                    ApplicationMethod = "Broadcasting"
                });
                actualNutrients.Nitrogen = targetNutrients.Nitrogen;
            }
        }

        if (targetNutrients.Phosphorus > 0)
        {
            var dap = availableProducts.FirstOrDefault(p => p.Name!.Contains("DAP") || p.PhosphorusContent >= 40);
            if (dap != null)
            {
                var quantityPerHa = targetNutrients.Phosphorus / (dap.PhosphorusContent / 100.0);
                recommendedProducts.Add(new RecommendedProduct
                {
                    ProductId = dap.Id,
                    ProductName = dap.Name!,
                    QuantityKgPerHa = quantityPerHa,
                    TotalQuantityKg = quantityPerHa * fieldAreaHa,
                    NutrientsProvided = new NutrientRequirement { Phosphorus = targetNutrients.Phosphorus },
                    ApplicationMethod = "Broadcasting"
                });
                actualNutrients.Phosphorus = targetNutrients.Phosphorus;
            }
        }

        if (targetNutrients.Potassium > 0)
        {
            var mop = availableProducts.FirstOrDefault(p => p.Name!.Contains("MOP") || p.PotassiumContent >= 50);
            if (mop != null)
            {
                var quantityPerHa = targetNutrients.Potassium / (mop.PotassiumContent / 100.0);
                recommendedProducts.Add(new RecommendedProduct
                {
                    ProductId = mop.Id,
                    ProductName = mop.Name!,
                    QuantityKgPerHa = quantityPerHa,
                    TotalQuantityKg = quantityPerHa * fieldAreaHa,
                    NutrientsProvided = new NutrientRequirement { Potassium = targetNutrients.Potassium },
                    ApplicationMethod = "Broadcasting"
                });
                actualNutrients.Potassium = targetNutrients.Potassium;
            }
        }

        return new ProductRecommendation
        {
            Products = recommendedProducts,
            TargetNutrients = targetNutrients,
            ActualNutrients = actualNutrients,
            OptimizationStrategy = optimizationStrategy,
            TotalCost = 0 // Can be calculated if product prices are available
        };
    }

    #region Helper Methods

    private async Task<NutrientRequirement> CalculateAlreadyAppliedNutrientsAsync(int fieldId, DateTime sowingDate)
    {
        var records = await _repository.FertilizerApplicationRecordRepository
            .GetAllAsync(r =>
                r.FieldId == fieldId &&
                r.ApplicationDate >= sowingDate &&
                r.ApplicationDate <= DateTime.Now);

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

        return totalApplied;
    }

    private async Task<List<FertilizerApplication>> GenerateApplicationScheduleAsync(
        CropVariety crop,
        FertilizationPlan? fertPlan,
        NutrientRequirement remainingToApply,
        DateTime sowingDate,
        double fieldAreaHa)
    {
        var applications = new List<FertilizerApplication>();

        if (fertPlan == null || !fertPlan.Stages.Any())
            // Fallback to default schedule if no plan exists
            return GenerateDefaultSchedule(crop, remainingToApply, sowingDate, fieldAreaHa);

        foreach (var stage in fertPlan.Stages.OrderBy(s => s.TimingFactor))
        {
            var applicationDate = CalculateApplicationDate(crop, stage, sowingDate);
            var daysAfterPlanting = (applicationDate - sowingDate).Days;

            var nutrientsForStage = new NutrientRequirement
            {
                Nitrogen = remainingToApply.Nitrogen * stage.NitrogenPercent,
                Phosphorus = remainingToApply.Phosphorus * stage.PhosphorusPercent,
                Potassium = remainingToApply.Potassium * stage.PotassiumPercent,
                Sulfur = remainingToApply.Sulfur * stage.SulfurPercent,
                Calcium = remainingToApply.Calcium * stage.CalciumPercent,
                Magnesium = remainingToApply.Magnesium * stage.MagnesiumPercent,
                Boron = remainingToApply.Boron * stage.BoronPercent,
                Zinc = remainingToApply.Zinc * stage.ZincPercent,
                Manganese = remainingToApply.Manganese * stage.ManganesePercent,
                Copper = remainingToApply.Copper * stage.CopperPercent,
                Iron = remainingToApply.Iron * stage.IronPercent,
                Molybdenum = remainingToApply.Molybdenum * stage.MolybdenumPercent
            };

            applications.Add(new FertilizerApplication
            {
                RecommendedDate = applicationDate,
                CropStage = stage.StageName!,
                DaysAfterPlanting = daysAfterPlanting,
                NutrientsToApply = nutrientsForStage,
                ApplicationMethod = stage.ApplicationMethod?.Name ?? "Не визначено",
                Rationale = stage.Rationale ?? "",
                Products = new List<FertilizerProductDto>()
            });
        }

        return applications;
    }

    private List<FertilizerApplication> GenerateDefaultSchedule(
        CropVariety crop,
        NutrientRequirement totalNeeded,
        DateTime sowingDate,
        double fieldAreaHa)
    {
        var applications = new List<FertilizerApplication>();

        // Pre-planting
        applications.Add(new FertilizerApplication
        {
            RecommendedDate = sowingDate.AddDays(-3),
            CropStage = "Передпосівне",
            DaysAfterPlanting = -3,
            NutrientsToApply = new NutrientRequirement
            {
                Nitrogen = totalNeeded.Nitrogen * 0.15,
                Phosphorus = totalNeeded.Phosphorus * 1.0,
                Potassium = totalNeeded.Potassium * 0.30,
                Sulfur = totalNeeded.Sulfur * 0.5,
                Calcium = totalNeeded.Calcium * 0.5,
                Magnesium = totalNeeded.Magnesium * 0.5
            },
            ApplicationMethod = "Розкидання з загортанням",
            Rationale = "Базове внесення"
        });

        // Early vegetative
        var earlyVegDay = crop.LIni + crop.LDev / 3;
        applications.Add(new FertilizerApplication
        {
            RecommendedDate = sowingDate.AddDays(earlyVegDay),
            CropStage = "Рання вегетація",
            DaysAfterPlanting = earlyVegDay,
            NutrientsToApply = new NutrientRequirement
            {
                Nitrogen = totalNeeded.Nitrogen * 0.35,
                Potassium = totalNeeded.Potassium * 0.25,
                Sulfur = totalNeeded.Sulfur * 0.25
            },
            ApplicationMethod = "Підживлення",
            Rationale = "Підтримка росту"
        });

        // Mid-season
        var midSeasonDay = crop.LIni + crop.LDev + crop.LMid / 3;
        applications.Add(new FertilizerApplication
        {
            RecommendedDate = sowingDate.AddDays(midSeasonDay),
            CropStage = "Середина сезону",
            DaysAfterPlanting = midSeasonDay,
            NutrientsToApply = new NutrientRequirement
            {
                Nitrogen = totalNeeded.Nitrogen * 0.40,
                Potassium = totalNeeded.Potassium * 0.35,
                Sulfur = totalNeeded.Sulfur * 0.25
            },
            ApplicationMethod = "Листкове підживлення",
            Rationale = "Формування врожаю"
        });

        // Late season (if needed)
        if (totalNeeded.Nitrogen * 0.10 > 5)
        {
            var lateSeasonDay = crop.LIni + crop.LDev + crop.LMid + crop.LLate / 4;
            applications.Add(new FertilizerApplication
            {
                RecommendedDate = sowingDate.AddDays(lateSeasonDay),
                CropStage = "Пізній сезон",
                DaysAfterPlanting = lateSeasonDay,
                NutrientsToApply = new NutrientRequirement
                {
                    Nitrogen = totalNeeded.Nitrogen * 0.10,
                    Potassium = totalNeeded.Potassium * 0.10
                },
                ApplicationMethod = "Листкове обприскування",
                Rationale = "Налив зерна"
            });
        }

        return applications;
    }

    private DateTime CalculateApplicationDate(
        CropVariety crop,
        PlanStage stage,
        DateTime sowingDate)
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

    private string DetermineGrowthStage(CropVariety crop, int daysAfterPlanting)
    {
        if (daysAfterPlanting < 0) return "Pre-planting";
        if (daysAfterPlanting <= crop.LIni) return "Initial";
        if (daysAfterPlanting <= crop.LIni + crop.LDev) return "Development";
        if (daysAfterPlanting <= crop.LIni + crop.LDev + crop.LMid) return "Mid-Season";
        if (daysAfterPlanting <= crop.GrowingDuration) return "Late-Season";
        return "Post-harvest";
    }

    private string DeterminePriority(int daysAfterPlanting, string currentStage, CropVariety crop)
    {
        if (currentStage == "Development" || currentStage == "Mid-Season")
            return "High";
        if (currentStage == "Initial")
            return "Medium";
        return "Low";
    }

    private List<string> GenerateWarnings(Field field, string currentStage, int daysToHarvest)
    {
        var warnings = new List<string>();

        if (daysToHarvest < 14)
            warnings.Add("Близько до збору врожаю. Уникайте внесення азоту.");

        if (field.Conditions?.FirstOrDefault()?.SoilMoisture < 0.2)
            warnings.Add("Низька вологість грунту може знизити ефективність добрив.");

        return warnings;
    }

    private List<string> GenerateDeficitRecommendations(List<NutrientDeficit> deficits)
    {
        var recommendations = new List<string>();

        foreach (var deficit in deficits.Where(d => d.Urgency == "High"))
            recommendations.Add($"Терміново внести {deficit.DeficitAmount:F1} кг/га {deficit.NutrientName}");

        return recommendations;
    }

    private string DetermineOverallNutrientStatus(NutrientRequirement deficit)
    {
        if (deficit.Nitrogen > 50 || deficit.Phosphorus > 30 || deficit.Potassium > 40)
            return "Deficient";
        if (deficit.Nitrogen < 10 && deficit.Phosphorus < 5 && deficit.Potassium < 10)
            return "Optimal";
        return "Moderate";
    }

    private List<string> GenerateBalanceRecommendations(NutrientRequirement deficit, int daysToHarvest)
    {
        var recommendations = new List<string>();

        if (deficit.Nitrogen > 20 && daysToHarvest > 30)
            recommendations.Add($"Рекомендується додаткове внесення азоту: {deficit.Nitrogen:F1} кг/га");

        if (deficit.Phosphorus > 15)
            recommendations.Add($"Недостатньо фосфору: {deficit.Phosphorus:F1} кг/га P2O5");

        if (deficit.Potassium > 20)
            recommendations.Add($"Недостатньо калію: {deficit.Potassium:F1} кг/га K2O");

        return recommendations;
    }

    private List<string> GenerateBalanceWarnings(NutrientRequirement deficit, NutrientRequirement surplus)
    {
        var warnings = new List<string>();

        if (surplus.Nitrogen > 30)
            warnings.Add("Надлишок азоту може призвести до вилягання культури");

        if (deficit.Phosphorus > 50)
            warnings.Add("Критичний дефіцит фосфору може знизити врожайність на 20-30%");

        return warnings;
    }

    private NutrientRequirement SubtractNutrients(NutrientRequirement from, NutrientRequirement subtract)
    {
        return new NutrientRequirement
        {
            Nitrogen = Math.Max(0, from.Nitrogen - subtract.Nitrogen),
            Phosphorus = Math.Max(0, from.Phosphorus - subtract.Phosphorus),
            Potassium = Math.Max(0, from.Potassium - subtract.Potassium),
            Sulfur = Math.Max(0, from.Sulfur - subtract.Sulfur),
            Calcium = Math.Max(0, from.Calcium - subtract.Calcium),
            Magnesium = Math.Max(0, from.Magnesium - subtract.Magnesium),
            Boron = Math.Max(0, from.Boron - subtract.Boron),
            Zinc = Math.Max(0, from.Zinc - subtract.Zinc),
            Manganese = Math.Max(0, from.Manganese - subtract.Manganese),
            Copper = Math.Max(0, from.Copper - subtract.Copper),
            Iron = Math.Max(0, from.Iron - subtract.Iron),
            Molybdenum = Math.Max(0, from.Molybdenum - subtract.Molybdenum)
        };
    }

    private double GetSoilEfficiencyFactor(SoilType type)
    {
        return type switch
        {
            SoilType.Loamy => 0.9,
            SoilType.Clay => 0.75,
            SoilType.Sandy => 0.6,
            SoilType.Silty => 0.85,
            SoilType.Peaty => 0.8,
            _ => 0.7
        };
    }

    #endregion
}
