using FluentValidation;
using SmartAgroPlan.DAL.Entities.Calendar;

namespace SmartAgroPlan.BLL.Validators.Calendar;

public class DayMonthValidator : AbstractValidator<DayMonth>
{
    private static readonly Dictionary<int, int> DaysInMonth = new()
    {
        [1] = 31,
        [2] = 29,
        [3] = 31,
        [4] = 30,
        [5] = 31,
        [6] = 30,
        [7] = 31,
        [8] = 31,
        [9] = 30,
        [10] = 31,
        [11] = 30,
        [12] = 31
    };

    public DayMonthValidator()
    {
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("Month must be between 1 and 12.");

        RuleFor(x => x.Day)
            .Must((dayMonth, day) => IsValidDayForMonth(day, dayMonth.Month))
            .WithMessage(dayMonth => $"Day must be between 1 and {MaxDaysInMonth(dayMonth.Month)} for month {dayMonth.Month}.");
    }
    private static bool IsValidDayForMonth(int day, int month) =>
        day >= 1 && DaysInMonth.TryGetValue(month, out var maxDay) && day <= maxDay;

    private static int MaxDaysInMonth(int month) => DaysInMonth.GetValueOrDefault(month, 0);
}
