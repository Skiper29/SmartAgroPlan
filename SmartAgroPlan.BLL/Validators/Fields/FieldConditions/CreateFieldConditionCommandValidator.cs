using FluentValidation;
using SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.Create;

namespace SmartAgroPlan.BLL.Validators.Fields.FieldConditions;

public class CreateFieldConditionCommandValidator : AbstractValidator<CreateFieldConditionCommand>
{
    public CreateFieldConditionCommandValidator(BaseFieldConditionValidator baseFieldConditionValidator)
    {
        RuleFor(x => x.NewFieldCondition)
            .NotNull()
            .WithMessage("Новий об'єкт умови поля не може бути null")
            .SetValidator(baseFieldConditionValidator);
    }
}
