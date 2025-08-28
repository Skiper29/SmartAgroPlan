namespace SmartAgroPlan.BLL.DTO.Fields.FieldCropHistory;

public class FieldCropHistoryCreateUpdateDto
{
    public int FieldId { get; set; }
    public int CropId { get; set; }
    public DateOnly? PlantedDate { get; set; }
    public DateOnly? HarvestedDate { get; set; }
    public double? Yield { get; set; }
    public string? Notes { get; set; }
}