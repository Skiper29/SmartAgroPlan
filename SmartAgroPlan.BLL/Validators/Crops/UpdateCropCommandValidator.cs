using FluentValidation;
using SmartAgroPlan.BLL.MediatR.Crops.Update;

namespace SmartAgroPlan.BLL.Validators.Crops;

public class UpdateCropCommandValidator : AbstractValidator<UpdateCropCommand>
{
    public UpdateCropCommandValidator(BaseCropVarietyValidator baseCropVarietyValidator)
    {
        RuleFor(x => x.CropVariety)
            .NotNull().WithMessage("Updated CropVariety object cannot be null")
            .SetValidator(baseCropVarietyValidator);

        RuleFor(x => x.CropVariety.Id)
            .GreaterThan(0)
            .WithMessage("Id must be a positive integer.");
    }
}
