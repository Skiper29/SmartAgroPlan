using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.DTO.Fields.Field;

public class FieldCreateUpdateDto
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public DateTime? SowingDate { get; set; }
    public string? BoundaryGeoJson { get; set; }
    public FieldType FieldType { get; set; }
    public int? CurrentCropId { get; set; }
    public int SoilId { get; set; }
}
