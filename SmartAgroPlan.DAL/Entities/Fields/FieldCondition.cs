using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAgroPlan.DAL.Entities.Fields;

public class FieldCondition
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public int FieldId { get; set; }

    public Field? Field { get; set; }

    [Required] public DateTime RecordedAt { get; set; }

    // Основні параметри ґрунту
    public double? SoilMoisture { get; set; } // %
    public double? SoilPh { get; set; } // рівень pH
    public double? Nitrogen { get; set; } // N (kg/ha)
    public double? Phosphorus { get; set; } // P (kg/ha)
    public double? Potassium { get; set; } // K (kg/ha)

    // Параметри погоди/середовища
    public double? Temperature { get; set; } // °C
    public double? Rainfall { get; set; } // мм

    [StringLength(300)] public string? Notes { get; set; }
}
