using FluentValidation;
using SmartAgroPlan.BLL.MediatR.Crops.Create;

namespace SmartAgroPlan.BLL.Validators.Crops;

public class CreateCropCommandValidator : AbstractValidator<CreateCropCommand>
{
    public CreateCropCommandValidator(BaseCropVarietyValidator baseCropVarietyValidator)
    {
        RuleFor(x => x.CropVariety)
            .NotNull().WithMessage("Новий об'єкт культури не може бути null")
            .SetValidator(baseCropVarietyValidator);
    }
}
