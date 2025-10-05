using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.Interfaces.Crops;

public interface ICropCoefficientService
{
    double GetKc(CropType cropType, DateTime plantingDate, DateTime targetDate);
}
