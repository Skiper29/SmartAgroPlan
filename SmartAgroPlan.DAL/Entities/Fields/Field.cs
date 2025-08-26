using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic.FileIO;
using NetTopologySuite.Geometries;
using SmartAgroPlan.DAL.Entities.Crops;

namespace SmartAgroPlan.DAL.Entities.Fields;

public class Field
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
    [StringLength(200)]
    public string? Location { get; set; }
    [Required]
    public Polygon? Boundary { get; set; }
    public FieldType FieldType { get; set; }
    public int CurrentCropId { get; set; }
    public int SoilId { get; set; }
    public Soil? Soil { get; set; }
    public CropVariety? CurrentCrop { get; set; }
    public ICollection<FieldCropHistory>? CropHistories { get; set; } = new List<FieldCropHistory>();
}
