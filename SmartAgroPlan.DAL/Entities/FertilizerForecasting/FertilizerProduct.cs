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
    public ProductForm Form { get; set; }

    // NPK content (percentage)
    public double NitrogenContent { get; set; } // N
    public double PhosphorusContent { get; set; } // P₂O₅
    public double PotassiumContent { get; set; } // K₂O

    // Secondary nutrients (percentage)
    public double? SulfurContent { get; set; } // SO₃
    public double? CalciumContent { get; set; } // CaO
    public double? MagnesiumContent { get; set; } // MgO

    // Micronutrients (percentage)
    public double? IronContent { get; set; } // Fe
    public double? ZincContent { get; set; } // Zn
    public double? BoronContent { get; set; } // B
    public double? ManganeseContent { get; set; } // Mn
    public double? CopperContent { get; set; } // Cu
    public double? MolybdenumContent { get; set; } // Mo

    [StringLength(500)] public string? Description { get; set; }

    [StringLength(100)] public string? Manufacturer { get; set; }

    // Navigation properties
    public ICollection<FertilizerApplicationProduct> ApplicationProducts { get; set; } =
        new List<FertilizerApplicationProduct>();

    public ICollection<FertilizerApplicationRecordProduct> RecordProducts { get; set; } =
        new List<FertilizerApplicationRecordProduct>();
}
