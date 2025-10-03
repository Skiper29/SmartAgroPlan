using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.FieldCondition;

namespace SmartAgroPlan.BLL.Validators.Fields.FieldCondition;

public class CreateFieldConditionValidator : AbstractValidator<FieldConditionCreateDto>
{
    public CreateFieldConditionValidator()
    {
        Include(new BaseFieldConditionValidator());
    }
}
