using System.Globalization;

namespace SmartAgroPlan.DAL.Helpers;

/// <summary>
/// Base class for CSV parsing with common utility methods
/// </summary>
public static class BaseCsvParser
{
    /// <summary>
    /// Handles quoted fields and commas inside quotes
    /// </summary>
    public static string[] SplitCsvLine(string line)
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

    /// <summary>
    /// Extracts a field from CSV line, handling quotes
    /// </summary>
    public static string GetField(string line, int start, int end)
    {
        var field = line.Substring(start, end - start).Trim();
        if (field.StartsWith("\"") && field.EndsWith("\""))
            field = field.Substring(1, field.Length - 2).Replace("\"\"", "\"");
        return field;
    }

    /// <summary>
    /// Splits CSV content into lines, removing empty lines
    /// </summary>
    public static string[] GetCsvLines(string csvContent)
    {
        return csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Parses a double value, returning 0 if parsing fails
    /// </summary>
    public static double ParseDoubleOrDefault(string value, double defaultValue = 0)
    {
        return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// Parses a nullable double value, returning null if empty or parsing fails
    /// </summary>
    public static double? ParseNullableDouble(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    /// <summary>
    /// Parses an int value, returning 0 if parsing fails
    /// </summary>
    public static int ParseIntOrDefault(string value, int defaultValue = 0)
    {
        return int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// Parses an enum value, returning default if parsing fails
    /// </summary>
    public static TEnum ParseEnumOrDefault<TEnum>(string value, TEnum defaultValue = default) where TEnum : struct
    {
        return Enum.TryParse<TEnum>(value, true, out var result)
            ? result
            : defaultValue;
    }
}
