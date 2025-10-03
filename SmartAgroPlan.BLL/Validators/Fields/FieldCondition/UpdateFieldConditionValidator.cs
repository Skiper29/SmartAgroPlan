using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.FieldCondition;

namespace SmartAgroPlan.BLL.Validators.Fields.FieldCondition;

public class UpdateFieldConditionValidator : AbstractValidator<FieldConditionUpdateDto>
{
    public UpdateFieldConditionValidator()
    {
        Include(new BaseFieldConditionValidator());

        RuleFor(fc => fc.Id)
            .GreaterThan(0)
            .WithMessage("Field Condition ID must be greater than 0.");
    }
}
