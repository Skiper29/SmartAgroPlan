using System.Globalization;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Helpers;

public static class CropVarietyCsvParser
{
    public static List<CropVariety> Parse(string csvContent, List<Soil> soils)
    {
        var result = new List<CropVariety>();
        var soilTypeToId = soils.ToDictionary(s => s.Type, s => s.Id);
        var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return result;

        for (var i = 1; i < lines.Length; i++)
        {
            var fields = SplitCsvLine(lines[i]);
            if (fields.Length < 14) continue;
            try
            {
                var name = fields[0];
                var cropType = Enum.Parse<CropType>(fields[1]);
                var waterRequirement = double.Parse(fields[2], CultureInfo.InvariantCulture);
                var fertilizerRequirement = double.Parse(fields[3], CultureInfo.InvariantCulture);
                var growingDuration = int.Parse(fields[4], CultureInfo.InvariantCulture);
                var lIni = int.Parse(fields[5], CultureInfo.InvariantCulture);
                var lDev = int.Parse(fields[6], CultureInfo.InvariantCulture);
                var lMid = int.Parse(fields[7], CultureInfo.InvariantCulture);
                var lLate = int.Parse(fields[8], CultureInfo.InvariantCulture);
                var sowingStart = new DayMonth(int.Parse(fields[9]), int.Parse(fields[10]));
                var sowingEnd = new DayMonth(int.Parse(fields[11]), int.Parse(fields[12]));
                var minTemperature = double.Parse(fields[13], CultureInfo.InvariantCulture);
                var maxTemperature = double.Parse(fields[14], CultureInfo.InvariantCulture);
                var harvestYield = double.Parse(fields[15], CultureInfo.InvariantCulture);
                var optimalSoilType = Enum.Parse<SoilType>(fields[16]);
                var additionalNotes = fields[17];
                var optimalSoilId = soilTypeToId.TryGetValue(optimalSoilType, out var id) ? id : 0;
                var cropVariety = new CropVariety
                {
                    Name = name,
                    CropType = cropType,
                    WaterRequirement = waterRequirement,
                    FertilizerRequirement = fertilizerRequirement,
                    GrowingDuration = growingDuration,
                    LIni = lIni,
                    LDev = lDev,
                    LMid = lMid,
                    LLate = lLate,
                    SowingStart = sowingStart,
                    SowingEnd = sowingEnd,
                    MinTemperature = minTemperature,
                    MaxTemperature = maxTemperature,
                    HarvestYield = harvestYield,
                    AdditionalNotes = additionalNotes,
                    OptimalSoilId = optimalSoilId
                };
                result.Add(cropVariety);
            }
            catch
            {
                // Ignore malformed lines
            }
        }

        return result;
    }

    // Handles quoted fields and commas inside quotes
    private static string[] SplitCsvLine(string line)
    {
        var fields = new List<string>();
        var inQuotes = false;
        var start = 0;
        for (var i = 0; i < line.Length; i++)
            if (line[i] == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (line[i] == ',' && !inQuotes)
            {
                fields.Add(GetField(line, start, i));
                start = i + 1;
            }

        fields.Add(GetField(line, start, line.Length));
        return fields.ToArray();
    }

    private static string GetField(string line, int start, int end)
    {
        var field = line.Substring(start, end - start);
        if (field.StartsWith("\"") && field.EndsWith("\""))
            field = field.Substring(1, field.Length - 2).Replace("\"\"", "\"");
        return field;
    }
}
