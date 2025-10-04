using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;

namespace SmartAgroPlan.BLL.Validators.Fields.FieldConditions;

public class CreateFieldConditionValidator : AbstractValidator<FieldConditionCreateDto>
{
    public CreateFieldConditionValidator()
    {
        Include(new BaseFieldConditionValidator());
    }
}
