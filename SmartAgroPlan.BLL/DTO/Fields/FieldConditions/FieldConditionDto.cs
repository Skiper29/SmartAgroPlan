namespace SmartAgroPlan.BLL.DTO.Fields.FieldConditions;

public class FieldConditionDto
{
    public int Id { get; set; }
    public int FieldId { get; set; }
    public DateTime RecordedAt { get; set; }
    public double? SoilMoisture { get; set; }
    public double? SoilPh { get; set; }
    public double? Nitrogen { get; set; }
    public double? Phosphorus { get; set; }
    public double? Potassium { get; set; }
    public double? Sulfur { get; set; }
    public double? Calcium { get; set; }
    public double? Magnesium { get; set; }
    public double? Temperature { get; set; }
    public double? Rainfall { get; set; }
    public string? Notes { get; set; }
}
