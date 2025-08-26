using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAgroPlan.DAL.Entities.Crops;

namespace SmartAgroPlan.DAL.Entities.Fields;

public class FieldCropHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int FieldId { get; set; }
    [Required]
    public int CropId { get; set; }
    [Required]
    public DateTime PlantedDate { get; set; }
    public DateTime? HarvestedDate { get; set; }
    public double? Yield { get; set; }
    [StringLength(300)]
    public string? Notes { get; set; }
    public Field? Field { get; set; }
    public CropVariety? Crop { get; set; }
}
