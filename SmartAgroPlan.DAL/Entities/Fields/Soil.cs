using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Entities.Fields;

public class Soil
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public SoilType Type { get; set; }
    public double WaterRetention { get; set; }      // water retention capacity in percentage
    public double Acidity { get; set; }             // pH level of the soil
    public double NutrientContent { get; set; }     // nutrient content in mg/kg
    public double OrganicMatter { get; set; }       // organic matter content in percentage
    public double SoilDensity { get; set; }         // soil density in g/cm³
    public double ErosionRisk { get; set; }         // erosion risk in percentage
    public ICollection<CropVariety>? OptimalCrops { get; set; } = new List<CropVariety>(); // Navigation property to CropVariety
    public ICollection<Field>? Fields { get; set; } = new List<Field>(); // Navigation property to Fields
}
