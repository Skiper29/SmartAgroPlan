using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.DTO.Fields.Soil;

public class SoilDto
{
    public int Id { get; set; }
    public SoilType Type { get; set; }
    public double WaterRetention { get; set; }
    public double Acidity { get; set; }
    public double NutrientContent { get; set; }
    public double OrganicMatter { get; set; }
    public double SoilDensity { get; set; }
    public double ErosionRisk { get; set; }
}
