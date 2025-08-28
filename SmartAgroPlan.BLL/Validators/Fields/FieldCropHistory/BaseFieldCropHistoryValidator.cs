using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.FieldCropHistory;

namespace SmartAgroPlan.BLL.Validators.Fields.FieldCropHistory;

public class BaseFieldCropHistoryValidator : AbstractValidator<FieldCropHistoryCreateUpdateDto>
{
    public const int MaxNotesLength = 300;

    public BaseFieldCropHistoryValidator()
    {
        RuleFor(fch => fch.FieldId)
            .GreaterThan(0)
            .WithMessage("FieldId must be a positive integer.");

        RuleFor(fch => fch.CropId)
            .GreaterThan(0)
            .WithMessage("CropId must be a positive integer.");

        RuleFor(fch => fch.PlantedDate)
            .NotNull()
            .WithMessage("PlantedDate is required.");

        RuleFor(fch => fch.HarvestedDate)
            .GreaterThan(fch => fch.PlantedDate)
            .When(fch => fch.HarvestedDate.HasValue)
            .WithMessage("HarvestedDate must be after PlantedDate.");

        RuleFor(fch => fch.Yield)
            .GreaterThanOrEqualTo(0)
            .When(fch => fch.Yield.HasValue)
            .WithMessage("Yield must be non-negative.");

        RuleFor(fch => fch.Notes)
            .MaximumLength(MaxNotesLength)
            .WithMessage($"Notes cannot exceed {MaxNotesLength} characters.");
    }
}
