using SmartAgroPlan.DAL.Entities.Crops;

namespace SmartAgroPlan.BLL.Interfaces.Crops;

public interface ICropCoefficientService
{
    double GetKc(CropCoefficientDefinition definition, DateTime plantingDate, DateTime targetDate);
}
