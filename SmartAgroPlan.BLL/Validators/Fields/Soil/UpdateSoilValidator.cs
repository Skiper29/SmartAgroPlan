using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.Soil;

namespace SmartAgroPlan.BLL.Validators.Fields.Soil;

public class UpdateSoilValidator : AbstractValidator<SoilUpdateDto>
{
    public UpdateSoilValidator(BaseSoilValidator baseSoilValidator)
    {
        Include(baseSoilValidator);

        RuleFor(s => s.Id)
            .GreaterThan(0)
            .WithMessage("Id must be a positive integer.");
    }
}
