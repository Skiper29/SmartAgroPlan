using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;

namespace SmartAgroPlan.BLL.Validators.Fields.FieldConditions;

public class BaseFieldConditionValidator : AbstractValidator<FieldConditionCreateDto>
{
    public const int MaxNotesLength = 300;
    public const double MinSoilMoisture = 0;
    public const double MaxSoilMoisture = 100;
    public const double MinSoilPh = 0;
    public const double MaxSoilPh = 14;
    public const double MinNutrient = 0;
    public const double MinTemperature = -50;
    public const double MaxTemperature = 60;
    public const double MinRainfall = 0;

    public BaseFieldConditionValidator()
    {
        RuleFor(fc => fc.FieldId)
            .GreaterThan(0)
            .WithMessage("Field ID must be greater than 0.");

        // RecordedAt is optional
        When(fc => fc.RecordedAt != default, () =>
        {
            RuleFor(fc => fc.RecordedAt)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("Recorded date and time cannot be in the future.");
        });

        RuleFor(fc => fc.SoilMoisture)
            .InclusiveBetween(MinSoilMoisture, MaxSoilMoisture)
            .When(fc => fc.SoilMoisture.HasValue)
            .WithMessage($"Soil moisture must be between {MinSoilMoisture}% and {MaxSoilMoisture}%.");

        RuleFor(fc => fc.SoilPh)
            .InclusiveBetween(MinSoilPh, MaxSoilPh)
            .When(fc => fc.SoilPh.HasValue)
            .WithMessage($"Soil pH must be between {MinSoilPh} and {MaxSoilPh}.");

        RuleFor(fc => fc.Nitrogen)
            .GreaterThanOrEqualTo(MinNutrient)
            .When(fc => fc.Nitrogen.HasValue)
            .WithMessage($"Nitrogen must be greater than or equal to {MinNutrient} kg/ha.");

        RuleFor(fc => fc.Phosphorus)
            .GreaterThanOrEqualTo(MinNutrient)
            .When(fc => fc.Phosphorus.HasValue)
            .WithMessage($"Phosphorus must be greater than or equal to {MinNutrient} kg/ha.");

        RuleFor(fc => fc.Potassium)
            .GreaterThanOrEqualTo(MinNutrient)
            .When(fc => fc.Potassium.HasValue)
            .WithMessage($"Potassium must be greater than or equal to {MinNutrient} kg/ha.");

        RuleFor(fc => fc.Temperature)
            .InclusiveBetween(MinTemperature, MaxTemperature)
            .When(fc => fc.Temperature.HasValue)
            .WithMessage($"Temperature must be between {MinTemperature}°C and {MaxTemperature}°C.");

        RuleFor(fc => fc.Rainfall)
            .GreaterThanOrEqualTo(MinRainfall)
            .When(fc => fc.Rainfall.HasValue)
            .WithMessage($"Rainfall must be greater than or equal to {MinRainfall} mm.");

        RuleFor(fc => fc.Notes)
            .MaximumLength(MaxNotesLength)
            .When(fc => !string.IsNullOrEmpty(fc.Notes))
            .WithMessage($"Notes cannot exceed {MaxNotesLength} characters.");
    }
}
