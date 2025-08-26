using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartAgroPlan.DAL.Entities.Calendar;

namespace SmartAgroPlan.DAL.Persistence.Converters;
public class DayMonthConverter : ValueConverter<DayMonth, string>
{
    public DayMonthConverter() : base(
        v => $"{v.Day:D2}-{v.Month:D2}",
        v => ConvertToDayMonth(v))
    { }

    private static DayMonth ConvertToDayMonth(string v)
    {
        var parts = v.Split('-');
        return new DayMonth(int.Parse(parts[0]), int.Parse(parts[1]));
    }
}
