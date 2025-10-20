using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;
using SmartAgroPlan.BLL.Models.FertilizerForecasting;
using SmartAgroPlan.BLL.Models.Weather;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Entities.Irrigation;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.Services.FertilizerForecasting;

public class FertilizerCalculationService : IFertilizerCalculationService
{
    // Crop-specific nutrient uptake coefficients (kg/ton of yield)
    private readonly Dictionary<CropType, NutrientUptakeCoefficients> _cropCoefficients = new()
    {
        [CropType.Wheat] = new NutrientUptakeCoefficients
            { N = 20.0, P = 4.0, K = 5.0, S = 2.0, Ca = 1.5, Mg = 1.2 },
        [CropType.Corn] = new NutrientUptakeCoefficients
            { N = 22.0, P = 4.5, K = 6.5, S = 2.5, Ca = 2.0, Mg = 1.5 },
        [CropType.Barley] = new NutrientUptakeCoefficients
            { N = 18.0, P = 3.5, K = 4.5, S = 1.8, Ca = 1.3, Mg = 1.0 },
        [CropType.Sunflower] = new NutrientUptakeCoefficients
            { N = 25.0, P = 6.0, K = 12.0, S = 3.0, Ca = 3.5, Mg = 2.0 },
        [CropType.Soy] = new NutrientUptakeCoefficients
            { N = 15.0, P = 5.0, K = 15.0, S = 2.2, Ca = 2.5, Mg = 1.8 },
        [CropType.Rapeseed] = new NutrientUptakeCoefficients
            { N = 30.0, P = 7.0, K = 10.0, S = 5.0, Ca = 4.0, Mg = 2.5 },
        [CropType.Potato] = new NutrientUptakeCoefficients
            { N = 4.5, P = 0.8, K = 6.0, S = 0.5, Ca = 0.3, Mg = 0.4 },
        [CropType.SugarBeet] = new NutrientUptakeCoefficients
            { N = 4.0, P = 1.0, K = 5.0, S = 0.6, Ca = 1.0, Mg = 0.5 },
        [CropType.Tomato] = new NutrientUptakeCoefficients
            { N = 3.0, P = 0.6, K = 4.5, S = 0.4, Ca = 1.2, Mg = 0.3 },
        [CropType.Another] = new NutrientUptakeCoefficients
            { N = 20.0, P = 5.0, K = 7.0, S = 2.0, Ca = 2.0, Mg = 1.5 }
    };

    // Nutrient uptake timing curves (% of total uptake by growth stage)
    private readonly Dictionary<string, NutrientUptakeTiming> _uptakeTimings = new()
    {
        ["Initial"] = new NutrientUptakeTiming { N = 0.10, P = 0.15, K = 0.10 },
        ["Development"] = new NutrientUptakeTiming { N = 0.30, P = 0.30, K = 0.25 },
        ["Mid-Season"] = new NutrientUptakeTiming { N = 0.45, P = 0.40, K = 0.50 },
        ["Late-Season"] = new NutrientUptakeTiming { N = 0.15, P = 0.15, K = 0.15 }
    };

    public SeasonFertilizerPlan CalculateSeasonPlan(CropVariety crop,
        Soil soil, FieldCondition currentCondition,
        FieldWithAreaDto field,
        double targetYield,
        DateTime sowingDate)
    {
        var plan = new SeasonFertilizerPlan
        {
            FieldId = field.Id,
            CropName = crop.Name,
            PlanGeneratedDate = DateTime.Now,
            ExpectedYield = targetYield,
            FieldAreaHa = field.AreaInHectares,
            Applications = []
        };

        // Step 1: Calculate total season nutrient requirement based on yield
        plan.TotalSeasonRequirement = CalculateCropNutrientDemand(crop.CropType, targetYield);

        // Step 2: Estimate soil nutrient supply
        plan.SoilSupply = EstimateSoilSupply(soil, currentCondition, crop.GrowingDuration);

        // Step 3: Calculate fertilizer requirement
        plan.RequiredFromFertilizer = new NutrientRequirement
        {
            Nitrogen = Math.Max(0, plan.TotalSeasonRequirement.Nitrogen - plan.SoilSupply.Nitrogen),
            Phosphorus = Math.Max(0, plan.TotalSeasonRequirement.Phosphorus - plan.SoilSupply.Phosphorus),
            Potassium = Math.Max(0, plan.TotalSeasonRequirement.Potassium - plan.SoilSupply.Potassium),
            Sulfur = Math.Max(0, plan.TotalSeasonRequirement.Sulfur - plan.SoilSupply.Sulfur),
            Calcium = Math.Max(0, plan.TotalSeasonRequirement.Calcium - plan.SoilSupply.Calcium),
            Magnesium = Math.Max(0, plan.TotalSeasonRequirement.Magnesium - plan.SoilSupply.Magnesium)
        };

        // Step 4: Split into multiple applications based on growth stages
        plan.Applications = SplitIntoApplications(
            crop,
            plan.RequiredFromFertilizer,
            sowingDate,
            plan.FieldAreaHa
        );

        // Step 5: Generate notes
        plan.Notes = GeneratePlanNotes(plan);

        return plan;
    }

    public CurrentRecommendation CalculateCurrentRecommendation(Field field, CropVariety crop,
        FieldCondition latestCondition,
        DateTime sowingDate, List<IrrigationRecommendation> recentIrrigation, List<WeatherData> weatherForecast = null)
    {
        throw new NotImplementedException();
    }

    private NutrientRequirement CalculateCropNutrientDemand(CropType cropType, double yieldTonnesPerHa)
    {
        var coefficients = _cropCoefficients[cropType];
        return new NutrientRequirement
        {
            Nitrogen = coefficients.N * yieldTonnesPerHa,
            Phosphorus = coefficients.P * yieldTonnesPerHa * 2.29, // Convert P to P2O5
            Potassium = coefficients.K * yieldTonnesPerHa * 1.2, // Convert K to K2O
            Sulfur = coefficients.S * yieldTonnesPerHa,
            Calcium = coefficients.Ca * yieldTonnesPerHa,
            Magnesium = coefficients.Mg * yieldTonnesPerHa
        };
    }

    private NutrientRequirement EstimateSoilSupply(Soil soil, FieldCondition condition, int growingDays)
    {
        // Base supply from soil reserves
        var nSupply = condition.Nitrogen ?? 0;
        var pSupply = condition.Phosphorus ?? 0;
        var kSupply = condition.Potassium ?? 0;

        // Add mineralization contribution (organic matter releases nutrients)
        var mineralizationRate = soil.OrganicMatter * 20.0; // kg N/ha per season
        nSupply += mineralizationRate * (growingDays / 120.0);

        // Adjust for soil type efficiency
        var soilEfficiency = GetSoilEfficiencyFactor(soil.Type);

        return new NutrientRequirement
        {
            Nitrogen = nSupply * soilEfficiency,
            Phosphorus = pSupply * soilEfficiency,
            Potassium = kSupply * soilEfficiency,
            Sulfur = 10 * soilEfficiency, // Base soil sulfur
            Calcium = 50 * soilEfficiency,
            Magnesium = 30 * soilEfficiency
        };
    }

    private List<FertilizerApplication> SplitIntoApplications(
        CropVariety crop,
        NutrientRequirement totalNeeded,
        DateTime sowingDate,
        double fieldAreaHa)
    {
        var applications = new List<FertilizerApplication>();

        // Application 1: Pre-planting or at planting (basal)
        applications.Add(
            new FertilizerApplication
            {
                RecommendedDate = sowingDate.AddDays(-3),
                CropStage = "Передпосівне (Базове)",
                DaysAfterPlanting = -3,
                NutrientsToApply = new NutrientRequirement
                {
                    Nitrogen = totalNeeded.Nitrogen * 0.15,
                    Phosphorus = totalNeeded.Phosphorus * 1.0, // All P at planting
                    Potassium = totalNeeded.Potassium * 0.30,
                    Sulfur = totalNeeded.Sulfur * 0.5,
                    Calcium = totalNeeded.Calcium * 0.5,
                    Magnesium = totalNeeded.Magnesium * 0.5
                },
                ApplicationMethod = "Розкидання з загортанням",
                Rationale = "Внесення фосфору та базових елементів при сівбі для розвитку кореневої системи"
            });

        // Application 2: Early vegetative (tillering/emergence)
        var earlyVegDay = crop.LIni + crop.LDev / 3;
        applications.Add(
            new FertilizerApplication
            {
                RecommendedDate = sowingDate.AddDays(earlyVegDay),
                CropStage = "Рання вегетація",
                DaysAfterPlanting = earlyVegDay,
                NutrientsToApply = new NutrientRequirement
                {
                    Nitrogen = totalNeeded.Nitrogen * 0.3,
                    Potassium = totalNeeded.Potassium * 0.25,
                    Sulfur = totalNeeded.Sulfur * 0.25
                },
                ApplicationMethod = "Прикореневе або поверхневе підживлення",
                Rationale = "Підтримка швидкого вегетативного росту та накопичення біомаси"
            });

        // Application 3: Mid-season (flowering/heading)
        var midSeasonDay = crop.LIni + crop.LDev + crop.LMid / 3;
        applications.Add(
            new FertilizerApplication
            {
                RecommendedDate = sowingDate.AddDays(midSeasonDay),
                CropStage = "Середина сезону (Цвітіння)",
                DaysAfterPlanting = midSeasonDay,
                NutrientsToApply = new NutrientRequirement
                {
                    Nitrogen = totalNeeded.Nitrogen * 0.4,
                    Potassium = totalNeeded.Potassium * 0.35,
                    Sulfur = totalNeeded.Sulfur * 0.25
                },
                ApplicationMethod = "Листкове підживлення або фертигація (за наявності)",
                Rationale = "Пікова потреба в елементах для репродуктивного розвитку та формування врожаю"
            });

        // Application 4: Late season (grain filling) - only for high-demand crops
        if (crop.CropType is CropType.Corn or CropType.Wheat)
        {
            var lateSeasonDay = crop.LIni + crop.LDev + crop.LMid + crop.LLate / 4;
            applications.Add(new FertilizerApplication
            {
                RecommendedDate = sowingDate.AddDays(lateSeasonDay),
                CropStage = "Пізній сезон (Налив зерна)",
                DaysAfterPlanting = lateSeasonDay,
                NutrientsToApply = new NutrientRequirement
                {
                    Nitrogen = totalNeeded.Nitrogen * 0.15,
                    Potassium = totalNeeded.Potassium * 0.10
                },
                ApplicationMethod = "Листкове обприскування",
                Rationale = "Підтримка наливу зерна та вмісту білка"
            });
        }

        // Add fertilizer products for each application
        foreach (var app in applications) app.Products = SelectFertilizerProducts(app.NutrientsToApply, fieldAreaHa);

        return applications;
    }

    private List<FertilizerProduct> SelectFertilizerProducts(NutrientRequirement needed, double areaHa)
    {
        var products = new List<FertilizerProduct>();

        // Urea for nitrogen (46-0-0)
        if (needed.Nitrogen > 0)
        {
            var ureaKgPerHa = needed.Nitrogen / 0.46;
            products.Add(new FertilizerProduct
            {
                Name = "Urea",
                NPK_N = 46, NPK_P = 0, NPK_K = 0,
                QuantityKgPerHa = ureaKgPerHa,
                TotalQuantityKg = ureaKgPerHa * areaHa
            });
        }

        // Diammonium phosphate (DAP) for phosphorus (18-46-0)
        if (needed.Phosphorus > 0)
        {
            var dapKgPerHa = needed.Phosphorus / 0.46;
            products.Add(new FertilizerProduct
            {
                Name = "Diammonium Phosphate (DAP)",
                NPK_N = 18, NPK_P = 46, NPK_K = 0,
                QuantityKgPerHa = dapKgPerHa,
                TotalQuantityKg = dapKgPerHa * areaHa
            });
        }

        // Potassium chloride (MOP) for potassium (0-0-60)
        if (needed.Potassium > 0)
        {
            var mopKgPerHa = needed.Potassium / 0.60;
            products.Add(new FertilizerProduct
            {
                Name = "MOP (Muriate of Potash)",
                NPK_N = 0, NPK_P = 0, NPK_K = 60,
                QuantityKgPerHa = mopKgPerHa,
                TotalQuantityKg = mopKgPerHa * areaHa
            });
        }

        return products;
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

    private string GeneratePlanNotes(SeasonFertilizerPlan plan)
    {
        return $"Total N required: {plan.RequiredFromFertilizer.Nitrogen:F1} kg/ha. " +
               $"Split into {plan.Applications.Count} applications for optimal uptake efficiency. " +
               $"Monitor soil conditions and adjust timing based on weather.";
    }
}
