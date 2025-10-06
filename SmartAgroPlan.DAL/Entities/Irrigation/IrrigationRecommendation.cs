namespace SmartAgroPlan.DAL.Entities.Irrigation;

public class IrrigationRecommendation
{
    public int Id { get; set; }
    public int FieldId { get; set; }
    public DateTime Date { get; set; }
    public double ET0 { get; set; }
    public double Kc { get; set; }
    public double ETc { get; set; }
    public double EffectivePrecipitation { get; set; }
    public double NetIrrigationRequirement { get; set; }
    public double GrossIrrigationRequirement { get; set; }
    public double? CurrentSoilMoisture { get; set; }
    public string RecommendedAction { get; set; }
    public string CropStage { get; set; }
    public string? Notes { get; set; }
}
