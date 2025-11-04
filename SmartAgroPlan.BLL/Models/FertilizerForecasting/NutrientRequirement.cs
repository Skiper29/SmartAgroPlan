namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

public class NutrientRequirement
{
    // Macronutrients (kg/ha)
    public double Nitrogen { get; set; } // N
    public double Phosphorus { get; set; } // P2O5
    public double Potassium { get; set; } // K2O

    // Secondary nutrients (kg/ha)
    public double Sulfur { get; set; } // S
    public double Calcium { get; set; } // Ca
    public double Magnesium { get; set; } // Mg

    // Micronutrients (kg/ha)
    public double Boron { get; set; } // B
    public double Zinc { get; set; } // Zn
    public double Manganese { get; set; } // Mn
    public double Copper { get; set; } // Cu
    public double Iron { get; set; } // Fe
    public double Molybdenum { get; set; } // Mo
}
