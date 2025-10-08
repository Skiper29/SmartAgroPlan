using SmartAgroPlan.BLL.Interfaces.Irrigation;
using SmartAgroPlan.BLL.Models.Irrigation;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.Services.Irrigation;

public class SoilWaterService : ISoilWaterService
{
    public SoilWaterParameters GetSoilParams(SoilType soilType, CropType cropType)
    {
        // Базові параметри FC та PWP за типом ґрунту (об’ємна частка)
        var p = soilType switch
        {
            SoilType.Sandy => new SoilWaterParameters { FieldCapacity = 0.12, WiltingPoint = 0.05 },
            SoilType.Loamy => new SoilWaterParameters { FieldCapacity = 0.30, WiltingPoint = 0.14 },
            SoilType.Clay => new SoilWaterParameters { FieldCapacity = 0.40, WiltingPoint = 0.20 },
            SoilType.Peaty => new SoilWaterParameters { FieldCapacity = 0.60, WiltingPoint = 0.30 },
            SoilType.Silty => new SoilWaterParameters { FieldCapacity = 0.35, WiltingPoint = 0.15 },
            SoilType.Chalky => new SoilWaterParameters { FieldCapacity = 0.20, WiltingPoint = 0.07 },
            SoilType.Rocky => new SoilWaterParameters { FieldCapacity = 0.10, WiltingPoint = 0.03 },
            SoilType.Saline => new SoilWaterParameters { FieldCapacity = 0.25, WiltingPoint = 0.10 },
            _ => GetDefaultSoilParams()
        };

        // Додати MAD (Management Allowed Depletion) базово або на основі культури
        p.AllowableDepletionFraction = GetMADFraction(cropType);

        return p;
    }

    private double GetMADFraction(CropType crop)
    {
        // Прості умовні значення; можна тонко налаштовувати на культурні типи або фази
        return crop switch
        {
            CropType.Wheat => 0.50, // дозволяється виснаження до 50 % доступної води
            CropType.Corn => 0.60,
            CropType.Sunflower => 0.55,
            CropType.Potato => 0.3,
            CropType.Tomato => 0.40,
            CropType.Soy => 0.50,
            CropType.Rapeseed => 0.50,
            CropType.SugarBeet => 0.50,
            CropType.Barley => 0.50,
            CropType.Oats => 0.50,
            CropType.Rye => 0.50,
            CropType.Another => 0.50,
            _ => 0.50
        };
    }

    public SoilWaterParameters GetDefaultSoilParams()
    {
        // Типові значення для середнього суглинкового ґрунту
        return new SoilWaterParameters { FieldCapacity = 0.3, WiltingPoint = 0.12, AllowableDepletionFraction = 0.5 };
    }
}
