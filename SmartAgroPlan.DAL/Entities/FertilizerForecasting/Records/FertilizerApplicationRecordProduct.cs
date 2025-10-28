using System.ComponentModel.DataAnnotations;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;

/// <summary>
///     Links actual products used in an application record
/// </summary>
public class FertilizerApplicationRecordProduct
{
    [Key] public int Id { get; set; }

    [Required] public int ApplicationRecordId { get; set; }

    [Required] public int FertilizerProductId { get; set; }

    public double QuantityUsedKg { get; set; }

    // Navigation properties
    public FertilizerApplicationRecord? ApplicationRecord { get; set; }
    public FertilizerProduct? FertilizerProduct { get; set; }
}
