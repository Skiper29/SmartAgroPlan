using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.DTO.Fields.Field;

namespace SmartAgroPlan.BLL.DTO.Fields.FieldCropHistory;

public class FieldCropHistoryDto
{
    public int Id { get; set; }
    public FieldDto? Field { get; set; }
    public CropVarietyDto? Crop { get; set; }
    public DateOnly PlantedDate { get; set; }
    public DateOnly? HarvestedDate { get; set; }
    public double? Yield { get; set; }
    public string? Notes { get; set; }
}