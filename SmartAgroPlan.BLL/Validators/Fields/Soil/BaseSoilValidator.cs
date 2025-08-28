using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.Soil;

namespace SmartAgroPlan.BLL.Validators.Fields.Soil;

public class BaseSoilValidator : AbstractValidator<SoilCreateUpdateDto>
{
    private const double MinAcidity = 3.5;
    private const double MaxAcidity = 9.5;
    private const int MaxPercentage = 100;
    private const int MinPercentage = 0;

    public BaseSoilValidator()
    {
        RuleFor(s => s.Type)
            .IsInEnum()
            .WithMessage("Invalid soil type.");

        RuleFor(s => s.WaterRetention)
            .InclusiveBetween(MinPercentage, MaxPercentage)
            .WithMessage($"Water retention must be between {MinPercentage} and {MaxPercentage} percent.");

        RuleFor(s => s.Acidity)
            .InclusiveBetween(MinAcidity, MaxAcidity)
            .WithMessage($"Acidity must be between {MinAcidity} and {MaxAcidity}.");

        RuleFor(s => s.NutrientContent)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Nutrient content must be non-negative.");

        RuleFor(s => s.OrganicMatter)
            .InclusiveBetween(MinPercentage, MaxPercentage)
            .WithMessage($"Organic matter must be between {MinPercentage} and {MaxPercentage} percent.");

        RuleFor(s => s.SoilDensity)
            .GreaterThan(0)
            .WithMessage("Soil density must be positive.");

        RuleFor(s => s.ErosionRisk)
            .InclusiveBetween(MinPercentage, MaxPercentage)
            .WithMessage($"Erosion risk must be between {MinPercentage} and {MaxPercentage} percent.");
    }
}
