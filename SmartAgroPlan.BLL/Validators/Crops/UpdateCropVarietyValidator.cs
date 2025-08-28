using FluentValidation;
using SmartAgroPlan.BLL.DTO.Crops;

namespace SmartAgroPlan.BLL.Validators.Crops;

public class UpdateCropVarietyValidator : AbstractValidator<CropVarietyUpdateDto>
{
    public UpdateCropVarietyValidator(BaseCropVarietyValidator baseCropVarietyValidator)
    {
        Include(baseCropVarietyValidator);
    }
}
