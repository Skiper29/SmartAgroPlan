using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.FieldCropHistory;

namespace SmartAgroPlan.BLL.Validators.Fields.FieldCropHistory;

public class CreateFieldCropHistoryValidator : AbstractValidator<FieldCropHistoryCreateDto>
{
    public CreateFieldCropHistoryValidator(BaseFieldCropHistoryValidator baseValidator)
    {
        Include(baseValidator);
    }
}
