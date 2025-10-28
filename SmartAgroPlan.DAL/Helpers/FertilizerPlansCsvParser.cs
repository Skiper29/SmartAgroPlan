using System.Globalization;
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

        var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return result;

        for (var i = 1; i < lines.Length; i++)
        {
            var fields = SplitCsvLine(lines[i]);
            if (fields.Length < 22) continue;

            try
            {
                var planId = int.Parse(fields[0], CultureInfo.InvariantCulture);
                var planName = fields[1];
                var planDescription = fields[2];
                var cropType = Enum.Parse<CropType>(fields[3]);
                var stageId = int.Parse(fields[4], CultureInfo.InvariantCulture);
                var stageName = fields[5];
                var rationale = fields[6];
                var timingBase = Enum.Parse<GrowthStage>(fields[7]);
                var timingFactor = double.Parse(fields[8], CultureInfo.InvariantCulture);
                var applicationMethodId = int.Parse(fields[9], CultureInfo.InvariantCulture);
                var nitrogenPercent = double.Parse(fields[10], CultureInfo.InvariantCulture);
                var phosphorusPercent = double.Parse(fields[11], CultureInfo.InvariantCulture);
                var potassiumPercent = double.Parse(fields[12], CultureInfo.InvariantCulture);
                var sulfurPercent = double.Parse(fields[13], CultureInfo.InvariantCulture);
                var calciumPercent = double.Parse(fields[14], CultureInfo.InvariantCulture);
                var magnesiumPercent = double.Parse(fields[15], CultureInfo.InvariantCulture);
                var boronPercent = double.Parse(fields[16], CultureInfo.InvariantCulture);
                var zincPercent = double.Parse(fields[17], CultureInfo.InvariantCulture);
                var manganesePercent = double.Parse(fields[18], CultureInfo.InvariantCulture);
                var copperPercent = double.Parse(fields[19], CultureInfo.InvariantCulture);
                var ironPercent = double.Parse(fields[20], CultureInfo.InvariantCulture);
                var molybdenumPercent = double.Parse(fields[21], CultureInfo.InvariantCulture);

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

    // Handles quoted fields and commas inside quotes
    private static string[] SplitCsvLine(string line)
    {
        var fields = new List<string>();
        var inQuotes = false;
        var start = 0;

        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (line[i] == ',' && !inQuotes)
            {
                fields.Add(GetField(line, start, i));
                start = i + 1;
            }
        }

        fields.Add(GetField(line, start, line.Length));
        return fields.ToArray();
    }

    private static string GetField(string line, int start, int end)
    {
        var field = line.Substring(start, end - start).Trim();
        if (field.StartsWith("\"") && field.EndsWith("\""))
            field = field.Substring(1, field.Length - 2).Replace("\"\"", "\"");
        return field;
    }
}
