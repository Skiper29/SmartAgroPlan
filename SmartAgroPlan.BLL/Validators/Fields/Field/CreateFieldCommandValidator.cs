using FluentValidation;
using SmartAgroPlan.BLL.MediatR.Fields.Field.Create;

namespace SmartAgroPlan.BLL.Validators.Fields.Field;

public class CreateFieldCommandValidator : AbstractValidator<CreateFieldCommand>
{
    public CreateFieldCommandValidator(BaseFieldValidator baseFieldValidator)
    {
        RuleFor(x => x.NewField)
            .NotNull().WithMessage("Новий об'єкт поля не може бути null")
            .SetValidator(baseFieldValidator);
    }
}
