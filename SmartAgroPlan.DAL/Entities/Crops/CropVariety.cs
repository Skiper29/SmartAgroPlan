using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Entities.Crops;

public class CropVariety
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] [MaxLength(100)] public string? Name { get; set; }

    public CropType CropType { get; set; }

    public double WaterRequirement { get; set; } // in mm per growing season
    public double FertilizerRequirement { get; set; } // in kg per hectare
    public int GrowingDuration { get; set; } // in days

    // Crop Growth Stages Duration (FAO-56)
    public int LIni { get; set; } // Initial stage duration in days
    public int LDev { get; set; } // Development stage duration in days
    public int LMid { get; set; } // Mid-Season stage duration in days
    public int LLate { get; set; } // Late-Season stage duration in days

    public DayMonth SowingStart { get; set; } // Optimal sowing start date
    public DayMonth SowingEnd { get; set; } // Optimal sowing end date
    public double MinTemperature { get; set; } // in °C
    public double MaxTemperature { get; set; } // in °C
    public double HarvestYield { get; set; } // in tons per hectare

    [MaxLength(500)] public string? AdditionalNotes { get; set; } // Additional notes or comments

    public int OptimalSoilId { get; set; } // Foreign key to Soil entity
    public Soil? OptimalSoil { get; set; } // Navigation property to Soil entity
    public ICollection<Field>? Fields { get; set; } = new List<Field>(); // Navigation property to Fields

    public ICollection<FieldCropHistory>? FieldCropHistories { get; set; } =
        new List<FieldCropHistory>(); // Navigation property to FieldCropHistories
}
