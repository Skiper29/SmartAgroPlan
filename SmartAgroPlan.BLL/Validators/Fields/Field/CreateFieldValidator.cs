using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.Field;

namespace SmartAgroPlan.BLL.Validators.Fields.Field;

public class CreateFieldValidator : AbstractValidator<FieldCreateDto>
{
    public CreateFieldValidator(BaseFieldValidator baseFieldValidator)
    {
        Include(baseFieldValidator);
    }
}
