using SmartAgroPlan.DAL.Entities.FertilizerForecasting;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Helpers;

public static class FertilizerPlansCsvParser
{
    public class FertilizerPlanData
    {
        public List<FertilizationPlan> Plans { get; set; } = new();
        public List<PlanStage> Stages { get; set; } = new();
        public List<ApplicationMethod> ApplicationMethods { get; set; } = new();
    }

    public static FertilizerPlanData Parse(string csvContent)
    {
        var result = new FertilizerPlanData();
        var planDict = new Dictionary<int, FertilizationPlan>();
        var appMethodDict = new Dictionary<int, ApplicationMethod>();

        var lines = BaseCsvParser.GetCsvLines(csvContent);
        if (lines.Length < 2)
            return result;

        // Skip header row
        for (var i = 1; i < lines.Length; i++)
        {
            var fields = BaseCsvParser.SplitCsvLine(lines[i]);
            if (fields.Length < 22)
                continue;

            try
            {
                var planId = BaseCsvParser.ParseIntOrDefault(fields[0]);
                var planName = fields[1];
                var planDescription = fields[2];
                var cropType = BaseCsvParser.ParseEnumOrDefault<CropType>(fields[3]);
                var stageId = BaseCsvParser.ParseIntOrDefault(fields[4]);
                var stageName = fields[5];
                var rationale = fields[6];
                var timingBase = BaseCsvParser.ParseEnumOrDefault<GrowthStage>(fields[7]);
                var timingFactor = BaseCsvParser.ParseDoubleOrDefault(fields[8]);
                var applicationMethodId = BaseCsvParser.ParseIntOrDefault(fields[9]);
                var nitrogenPercent = BaseCsvParser.ParseDoubleOrDefault(fields[10]);
                var phosphorusPercent = BaseCsvParser.ParseDoubleOrDefault(fields[11]);
                var potassiumPercent = BaseCsvParser.ParseDoubleOrDefault(fields[12]);
                var sulfurPercent = BaseCsvParser.ParseDoubleOrDefault(fields[13]);
                var calciumPercent = BaseCsvParser.ParseDoubleOrDefault(fields[14]);
                var magnesiumPercent = BaseCsvParser.ParseDoubleOrDefault(fields[15]);
                var boronPercent = BaseCsvParser.ParseDoubleOrDefault(fields[16]);
                var zincPercent = BaseCsvParser.ParseDoubleOrDefault(fields[17]);
                var manganesePercent = BaseCsvParser.ParseDoubleOrDefault(fields[18]);
                var copperPercent = BaseCsvParser.ParseDoubleOrDefault(fields[19]);
                var ironPercent = BaseCsvParser.ParseDoubleOrDefault(fields[20]);
                var molybdenumPercent = BaseCsvParser.ParseDoubleOrDefault(fields[21]);

                // Add or get FertilizationPlan
                if (!planDict.ContainsKey(planId))
                {
                    var plan = new FertilizationPlan
                    {
                        Id = planId,
                        Name = planName,
                        Description = planDescription,
                        CropType = cropType
                    };
                    planDict[planId] = plan;
                }

                // Create PlanStage
                var stage = new PlanStage
                {
                    Id = stageId,
                    StageName = stageName,
                    Rationale = rationale,
                    FertilizationPlanId = planId,
                    GrowthStage = timingBase,
                    TimingFactor = timingFactor,
                    ApplicationMethodId = applicationMethodId,
                    NitrogenPercent = nitrogenPercent,
                    PhosphorusPercent = phosphorusPercent,
                    PotassiumPercent = potassiumPercent,
                    SulfurPercent = sulfurPercent,
                    CalciumPercent = calciumPercent,
                    MagnesiumPercent = magnesiumPercent,
                    BoronPercent = boronPercent,
                    ZincPercent = zincPercent,
                    ManganesePercent = manganesePercent,
                    CopperPercent = copperPercent,
                    IronPercent = ironPercent,
                    MolybdenumPercent = molybdenumPercent
                };
                result.Stages.Add(stage);

                // Track application method IDs (we'll create them separately)
                if (!appMethodDict.ContainsKey(applicationMethodId))
                {
                    appMethodDict[applicationMethodId] = null!; // placeholder
                }
            }
            catch
            {
                // Ignore malformed lines
            }
        }

        result.Plans = planDict.Values.ToList();

        // Create ApplicationMethods with standard names
        result.ApplicationMethods = CreateApplicationMethods(appMethodDict.Keys.ToList());

        return result;
    }

    private static List<ApplicationMethod> CreateApplicationMethods(List<int> ids)
    {
        var methods = new List<ApplicationMethod>();
        var methodNames = new Dictionary<int, (string Name, string Description)>
        {
            { 1, ("Основне внесення", "Внесення добрив у ґрунт під оранку або культивацію") },
            { 2, ("Прикореневе підживлення", "Внесення добрив у ґрунт біля кореневої системи") },
            { 3, ("Розкидне підживлення", "Поверхневе розкидання добрив") },
            { 4, ("Листкове підживлення", "Внесення добрив шляхом обприскування листової поверхні") },
            { 5, ("Фертигація", "Внесення добрив через систему краплинного зрошення") }
        };

        foreach (var id in ids.OrderBy(x => x))
        {
            if (methodNames.TryGetValue(id, out var methodData))
            {
                methods.Add(new ApplicationMethod
                {
                    Id = id,
                    Name = methodData.Name,
                    Description = methodData.Description
                });
            }
        }

        return methods;
    }
}
