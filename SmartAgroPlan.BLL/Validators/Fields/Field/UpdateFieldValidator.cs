using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.Field;

namespace SmartAgroPlan.BLL.Validators.Fields.Field;

public class UpdateFieldValidator : AbstractValidator<FieldUpdateDto>
{
    public UpdateFieldValidator(BaseFieldValidator baseFieldValidator)
    {
        Include(baseFieldValidator);
    }
}
