namespace SmartAgroPlan.BLL.DTO.Fields.FieldCondition;

public class FieldConditionCreateUpdateDto
{
    public int FieldId { get; set; }
    public DateTime RecordedAt { get; set; }
    public double? SoilMoisture { get; set; }
    public double? SoilPh { get; set; }
    public double? Nitrogen { get; set; }
    public double? Phosphorus { get; set; }
    public double? Potassium { get; set; }
    public double? Temperature { get; set; }
    public double? Rainfall { get; set; }
    public string? Notes { get; set; }
}
