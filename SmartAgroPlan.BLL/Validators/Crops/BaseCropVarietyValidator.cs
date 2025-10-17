using FluentValidation;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.Validators.Calendar;
using SmartAgroPlan.DAL.Entities.Calendar;

namespace SmartAgroPlan.BLL.Validators.Crops;

public class BaseCropVarietyValidator : AbstractValidator<CropVarietyCreateUpdateDto>
{
    public const int MaxNameLength = 100;
    public const int MaxAdditionalNotesLength = 500;

    public BaseCropVarietyValidator(DayMonthValidator dayMonthValidator)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(MaxNameLength)
            .WithMessage($"Name cannot exceed {MaxNameLength} characters.");

        RuleFor(x => x.WaterRequirement)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Water requirement must be non-negative.");

        RuleFor(x => x.FertilizerRequirement)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Fertilizer requirement must be non-negative.");

        RuleFor(x => x.GrowingDuration)
            .GreaterThan(0)
            .WithMessage("Growing duration must be a positive integer.");

        // Validate crop growth stages (FAO-56)
        RuleFor(x => x.LIni)
            .GreaterThan(0)
            .WithMessage("Initial stage duration (LIni) must be a positive integer.");

        RuleFor(x => x.LDev)
            .GreaterThan(0)
            .WithMessage("Development stage duration (LDev) must be a positive integer.");

        RuleFor(x => x.LMid)
            .GreaterThan(0)
            .WithMessage("Mid-Season stage duration (LMid) must be a positive integer.");

        RuleFor(x => x.LLate)
            .GreaterThan(0)
            .WithMessage("Late-Season stage duration (LLate) must be a positive integer.");

        // Validate that sum of growth stages equals growing duration
        RuleFor(x => x)
            .Must(x => x.LIni + x.LDev + x.LMid + x.LLate == x.GrowingDuration)
            .WithMessage(
                "The sum of all growth stage durations (LIni + LDev + LMid + LLate) must equal the Growing Duration.");

        RuleFor(x => x.SowingStart)
            .NotNull()
            .WithMessage("Sowing start date is required.")
            .SetValidator(dayMonthValidator);

        RuleFor(x => x.SowingEnd)
            .NotNull()
            .WithMessage("Sowing end date is required.")
            .SetValidator(dayMonthValidator)
            .Must((dto, sowingEnd) => IsSowingEndAfterStart(dto.SowingStart, sowingEnd))
            .WithMessage("Sowing end date must be after or equal to sowing start date.");

        RuleFor(x => x.MinTemperature)
            .LessThanOrEqualTo(x => x.MaxTemperature)
            .WithMessage("Minimum temperature must be less than or equal to maximum temperature.");

        RuleFor(x => x.HarvestYield)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Harvest yield must be non-negative.");

        RuleFor(x => x.AdditionalNotes)
            .MaximumLength(MaxAdditionalNotesLength)
            .WithMessage($"Additional notes cannot exceed {MaxAdditionalNotesLength} characters.");
    }

    private static bool IsSowingEndAfterStart(DayMonth sowingStart, DayMonth sowingEnd)
    {
        if (sowingStart.Month < sowingEnd.Month) return true;

        if (sowingStart.Month == sowingEnd.Month) return sowingStart.Day <= sowingEnd.Day;

        return false;
    }
}
