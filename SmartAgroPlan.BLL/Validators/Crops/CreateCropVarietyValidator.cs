using FluentValidation;
using SmartAgroPlan.BLL.DTO.Crops;

namespace SmartAgroPlan.BLL.Validators.Crops;

public class CreateCropVarietyValidator : AbstractValidator<CropVarietyCreateDto>
{
    public CreateCropVarietyValidator(BaseCropVarietyValidator baseCropVarietyValidator)
    {
        RuleFor(dto => dto).SetValidator(baseCropVarietyValidator);
    }
}
