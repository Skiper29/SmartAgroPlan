using SmartAgroPlan.BLL.Models.Irrigation;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.Interfaces.Irrigation;

public interface ISoilWaterService
{
    SoilWaterParameters GetSoilParams(SoilType soilType, CropType cropType);
    double GetFieldCapacity(SoilType soilType, CropType cropType);
    double GetPermanentWiltingPoint(SoilType soilType, CropType cropType);
    double GetManagementAllowedDepletion(SoilType soilType, CropType cropType);
}
