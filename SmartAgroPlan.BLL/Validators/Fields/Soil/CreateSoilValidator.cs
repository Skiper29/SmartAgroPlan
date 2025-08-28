using FluentValidation;
using SmartAgroPlan.BLL.DTO.Fields.Soil;

namespace SmartAgroPlan.BLL.Validators.Fields.Soil;

public class CreateSoilValidator : AbstractValidator<SoilCreateDto>
{
    public CreateSoilValidator(BaseSoilValidator baseSoilValidator)
    {
        Include(baseSoilValidator);
    }
}
