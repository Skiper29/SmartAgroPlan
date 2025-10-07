namespace SmartAgroPlan.BLL.Models.Irrigation;

public class SoilWaterParameters
{
    public double FieldCapacity { get; set; } // об’ємна частка або fraction (0..1)
    public double WiltingPoint { get; set; } // об’ємна частка або fraction (0..1)
    public double AllowableDepletionFraction { get; set; } // MAD (частка 0..1)
}
