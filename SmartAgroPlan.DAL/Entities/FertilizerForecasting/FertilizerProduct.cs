using System.ComponentModel.DataAnnotations;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Applications;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting;

/// <summary>
///     Master list of available fertilizer products
/// </summary>
public class FertilizerProduct
{
    [Key] public int Id { get; set; }

    [Required] [StringLength(100)] public string? Name { get; set; }

    public FertilizerType Type { get; set; }

    // NPK content (percentage)
    public double NitrogenContent { get; set; }
    public double PhosphorusContent { get; set; }
    public double PotassiumContent { get; set; }

    // Secondary nutrients (percentage)
    public double? SulfurContent { get; set; }
    public double? CalciumContent { get; set; }
    public double? MagnesiumContent { get; set; }

    // Micronutrients (percentage)
    public double? IronContent { get; set; }
    public double? ZincContent { get; set; }
    public double? BoronContent { get; set; }
    public double? ManganeseCont { get; set; }
    public double? CopperContent { get; set; }
    public double? MolybdenumContent { get; set; }

    [StringLength(500)] public string? Description { get; set; }

    [StringLength(100)] public string? Manufacturer { get; set; }

    // Navigation properties
    public ICollection<FertilizerApplicationProduct> ApplicationProducts { get; set; } =
        new List<FertilizerApplicationProduct>();

    public ICollection<FertilizerApplicationRecordProduct> RecordProducts { get; set; } =
        new List<FertilizerApplicationRecordProduct>();
}
