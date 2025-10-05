using System.ComponentModel.DataAnnotations;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Entities.Crops;

public class CropCoefficientDefinition
{
    [Key] public int Id { get; set; }
    [Required] public CropType CropType { get; set; }
    public double KcIni { get; set; }
    public double KcMid { get; set; }
    public double KcEnd { get; set; }
    public int LIni { get; set; } // days
    public int LDev { get; set; } // days
    public int LMid { get; set; } // days
    public int LLate { get; set; } // days
}
