using FluentValidation;
using SmartAgroPlan.BLL.MediatR.Fields.Field.Update;

namespace SmartAgroPlan.BLL.Validators.Fields.Field;

public class UpdateFieldCommandValidator : AbstractValidator<UpdateFieldCommand>
{
    public UpdateFieldCommandValidator(BaseFieldValidator baseFieldValidator)
    {
        RuleFor(x => x.UpdatedField)
            .NotNull().WithMessage("Оновлений об'єкт поля не може бути null")
            .SetValidator(baseFieldValidator);
    }
}
