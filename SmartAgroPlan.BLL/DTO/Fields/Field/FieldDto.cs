using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.DTO.Fields.Field;

public class FieldDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? BoundaryGeoJson { get; set; }
    public FieldType FieldType { get; set; }
    public CropVarietyDto? CurrentCrop { get; set; }
    public SoilDto? Soil { get; set; }
}