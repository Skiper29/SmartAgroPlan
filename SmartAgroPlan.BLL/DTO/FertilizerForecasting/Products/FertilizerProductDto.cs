using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;

public class FertilizerProductDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public FertilizerType Type { get; set; }
    public double NitrogenContent { get; set; }
    public double PhosphorusContent { get; set; }
    public double PotassiumContent { get; set; }
    public double? SulfurContent { get; set; }
    public double? CalciumContent { get; set; }
    public double? MagnesiumContent { get; set; }
    public double? IronContent { get; set; }
    public double? ZincContent { get; set; }
    public double? BoronContent { get; set; }
    public double? ManganeseCont { get; set; }
    public double? CopperContent { get; set; }
    public double? MolybdenumContent { get; set; }
    public string? Description { get; set; }
    public string? Manufacturer { get; set; }
}