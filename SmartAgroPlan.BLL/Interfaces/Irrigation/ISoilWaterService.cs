using SmartAgroPlan.BLL.Models.Irrigation;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.Interfaces.Irrigation;

public interface ISoilWaterService
{
    SoilWaterParameters GetSoilParams(SoilType soilType, CropType cropType);
    SoilWaterParameters GetDefaultSoilParams();
}
