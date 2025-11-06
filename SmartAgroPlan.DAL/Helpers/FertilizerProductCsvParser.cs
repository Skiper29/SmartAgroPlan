using SmartAgroPlan.DAL.Entities.FertilizerForecasting;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Helpers;

/// <summary>
/// Parser for fertilizer products CSV data
/// </summary>
public static class FertilizerProductCsvParser
{
    public static List<FertilizerProduct> Parse(string csvContent)
    {
        var result = new List<FertilizerProduct>();
        var lines = BaseCsvParser.GetCsvLines(csvContent);

        if (lines.Length < 2)
            return result;

        // Skip header row
        for (var i = 1; i < lines.Length; i++)
        {
            var fields = BaseCsvParser.SplitCsvLine(lines[i]);
            if (fields.Length < 18)
                continue;

            try
            {
                var product = new FertilizerProduct
                {
                    Id = BaseCsvParser.ParseIntOrDefault(fields[0]),
                    Name = fields[1],
                    Type = BaseCsvParser.ParseEnumOrDefault<FertilizerType>(fields[2]),
                    Form = BaseCsvParser.ParseEnumOrDefault<ProductForm>(fields[3]),
                    NitrogenContent = BaseCsvParser.ParseDoubleOrDefault(fields[4]),
                    PhosphorusContent = BaseCsvParser.ParseDoubleOrDefault(fields[5]),
                    PotassiumContent = BaseCsvParser.ParseDoubleOrDefault(fields[6]),
                    SulfurContent = BaseCsvParser.ParseNullableDouble(fields[7]),
                    CalciumContent = BaseCsvParser.ParseNullableDouble(fields[8]),
                    MagnesiumContent = BaseCsvParser.ParseNullableDouble(fields[9]),
                    IronContent = BaseCsvParser.ParseNullableDouble(fields[10]),
                    ZincContent = BaseCsvParser.ParseNullableDouble(fields[11]),
                    BoronContent = BaseCsvParser.ParseNullableDouble(fields[12]),
                    ManganeseContent = BaseCsvParser.ParseNullableDouble(fields[13]),
                    CopperContent = BaseCsvParser.ParseNullableDouble(fields[14]),
                    MolybdenumContent = BaseCsvParser.ParseNullableDouble(fields[15]),
                    Description = fields[16],
                    Manufacturer = fields[17]
                };

                result.Add(product);
            }
            catch
            {
                // Ignore malformed lines
            }
        }

        return result;
    }
}
