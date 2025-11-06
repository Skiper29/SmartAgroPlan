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
        var lines = BaseCsvParser.GetCsvLines(csvContent);

        if (lines.Length < 2)
            return result;

        // Skip header row
        for (var i = 1; i < lines.Length; i++)
        {
            var fields = BaseCsvParser.SplitCsvLine(lines[i]);
            if (fields.Length < 14)
                continue;

            try
            {
                var name = fields[0];
                var cropType = BaseCsvParser.ParseEnumOrDefault<CropType>(fields[1]);
                var waterRequirement = BaseCsvParser.ParseDoubleOrDefault(fields[2]);
                var fertilizerRequirement = BaseCsvParser.ParseDoubleOrDefault(fields[3]);
                var growingDuration = BaseCsvParser.ParseIntOrDefault(fields[4]);
                var lIni = BaseCsvParser.ParseIntOrDefault(fields[5]);
                var lDev = BaseCsvParser.ParseIntOrDefault(fields[6]);
                var lMid = BaseCsvParser.ParseIntOrDefault(fields[7]);
                var lLate = BaseCsvParser.ParseIntOrDefault(fields[8]);
                var sowingStart = new DayMonth(
                    BaseCsvParser.ParseIntOrDefault(fields[9]),
                    BaseCsvParser.ParseIntOrDefault(fields[10])
                );
                var sowingEnd = new DayMonth(
                    BaseCsvParser.ParseIntOrDefault(fields[11]),
                    BaseCsvParser.ParseIntOrDefault(fields[12])
                );
                var minTemperature = BaseCsvParser.ParseDoubleOrDefault(fields[13]);
                var maxTemperature = BaseCsvParser.ParseDoubleOrDefault(fields[14]);
                var harvestYield = BaseCsvParser.ParseDoubleOrDefault(fields[15]);
                var optimalSoilType = BaseCsvParser.ParseEnumOrDefault<SoilType>(fields[16]);
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
}
