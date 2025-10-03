using System.Diagnostics.CodeAnalysis;
using NetTopologySuite.Geometries;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Helpers;
using SmartAgroPlan.DAL.Persistence;

namespace SmartAgroPlan.WebAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class SeedingDataExtension
{
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SmartAgroPlanDbContext>();

        // Seed Soils
        if (!dbContext.Soils.Any())
        {
            var soils = new List<Soil>
            {
                new Soil {
                    Type = SoilType.Clay,
                    WaterRetention = 55.0,
                    Acidity = 7,
                    NutrientContent = 1500,
                    OrganicMatter = 4.0,
                    SoilDensity = 1.9,
                    ErosionRisk = 15.0
                },
                new Soil {
                    Type = SoilType.Sandy,
                    WaterRetention = 10.0,
                    Acidity = 6.0,
                    NutrientContent = 800,
                    OrganicMatter = 0.5,
                    SoilDensity = 1.6,
                    ErosionRisk = 70.0
                },
                new Soil {
                    Type = SoilType.Loamy,
                    WaterRetention = 35.0,
                    Acidity = 6.5,
                    NutrientContent = 1200,
                    OrganicMatter = 5.0,
                    SoilDensity = 1.35,
                    ErosionRisk = 30.0
                },
                new Soil {
                    Type = SoilType.Peaty,
                    WaterRetention = 75.0,
                    Acidity = 5,
                    NutrientContent = 800.0,
                    OrganicMatter = 30.0,
                    SoilDensity = 1.0,
                    ErosionRisk = 80
                },
                new Soil {
                    Type = SoilType.Saline,
                    WaterRetention = 25.0,
                    Acidity = 8.0,
                    NutrientContent = 600.0,
                    OrganicMatter = 1.5,
                    SoilDensity = 1.2,
                    ErosionRisk = 45.0
                },
                new Soil {
                    Type = SoilType.Chalky,
                    WaterRetention = 20.0,
                    Acidity = 8.5,
                    NutrientContent = 700.0,
                    OrganicMatter = 1.0,
                    SoilDensity = 1.4,
                    ErosionRisk = 50.0
                },
                new Soil {
                    Type = SoilType.Silty,
                    WaterRetention = 40.0,
                    Acidity = 6.8,
                    NutrientContent = 1300.0,
                    OrganicMatter = 3.0,
                    SoilDensity = 1.3,
                    ErosionRisk = 60.0
                },
                new Soil {
                    Type = SoilType.Rocky,
                    WaterRetention = 5.0,
                    Acidity = 7,
                    NutrientContent = 400.0,
                    OrganicMatter = 0.5,
                    SoilDensity = 1.8,
                    ErosionRisk = 90.0
                }
            };
            dbContext.Soils.AddRange(soils);
            await dbContext.SaveChangesAsync();
        }

        // Seed Crops
        if (!dbContext.Crops.Any())
        {
            // Read CSV file
            var baseDir = AppContext.BaseDirectory;
            var solutionDir = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
            var csvPath = Path.Combine(solutionDir, "Data", "crop_varieties_dataset.csv");
            List<CropVariety> crops;
            if (File.Exists(csvPath))
            {
                var csvContent = await File.ReadAllTextAsync(csvPath);
                var soils = dbContext.Soils.ToList();
                crops = CropVarietyCsvParser.Parse(csvContent, soils);
            }
            else
            {
                // fallback to default crops if CSV not found
                var soilLoamy = dbContext.Soils.First(s => s.Type == SoilType.Loamy);
                var soilClay = dbContext.Soils.First(s => s.Type == SoilType.Clay);
                var soilSandy = dbContext.Soils.First(s => s.Type == SoilType.Sandy);
                crops = new List<CropVariety>
                {
                    new CropVariety {
                        Name = "Wheat",
                        CropType = CropType.Wheat,
                        WaterRequirement = 500,
                        FertilizerRequirement = 120,
                        GrowingDuration = 120,
                        SowingStart = new DayMonth(15, 9),
                        SowingEnd = new DayMonth(30, 9),
                        MinTemperature = 5,
                        MaxTemperature = 30,
                        HarvestYield = 6.5,
                        OptimalSoilId = soilLoamy.Id,
                        AdditionalNotes = "Common wheat variety."
                    },
                    new CropVariety {
                        Name = "Corn",
                        CropType = CropType.Corn,
                        WaterRequirement = 600,
                        FertilizerRequirement = 150,
                        GrowingDuration = 100,
                        SowingStart = new DayMonth(1, 5),
                        SowingEnd = new DayMonth(15, 5),
                        MinTemperature = 10,
                        MaxTemperature = 35,
                        HarvestYield = 8.0,
                        OptimalSoilId = soilClay.Id,
                        AdditionalNotes = "High-yield corn."
                    },
                    new CropVariety {
                        Name = "Sunflower",
                        CropType = CropType.Sunflower,
                        WaterRequirement = 400,
                        FertilizerRequirement = 100,
                        GrowingDuration = 90,
                        SowingStart = new DayMonth(10, 4),
                        SowingEnd = new DayMonth(25, 4),
                        MinTemperature = 8,
                        MaxTemperature = 32,
                        HarvestYield = 3.2,
                        OptimalSoilId = soilSandy.Id,
                        AdditionalNotes = "Oilseed sunflower."
                    }
                };
            }
            dbContext.Crops.AddRange(crops);
            await dbContext.SaveChangesAsync();
        }

        // Seed Fields
        if (!dbContext.Fields.Any())
        {
            var fields = new List<Field>
            {
                new Field {
                    Name = "Field A",
                    Location = "North Farm",
                    Boundary = new Polygon(new LinearRing(new[] { new Coordinate(0,0), new Coordinate(0,1), new Coordinate(1,1), new Coordinate(1,0), new Coordinate(0,0) })),
                    FieldType = FieldType.Arable,
                    CurrentCropId = 1,
                    SoilId = 3,
                    SowingDate = new DateTime(2024, 9, 20,0,0,0,DateTimeKind.Utc)
                },
                new Field {
                    Name = "Field B",
                    Location = "East Farm",
                    Boundary = new Polygon(new LinearRing(new[] { new Coordinate(2,2), new Coordinate(2,3), new Coordinate(3,3), new Coordinate(3,2), new Coordinate(2,2) })),
                    FieldType = FieldType.Pasture,
                    CurrentCropId = 2,
                    SoilId = 1,
                    SowingDate = new DateTime(2024, 5, 5,0,0,0,DateTimeKind.Utc)
                },
                new Field {
                    Name = "Field C",
                    Location = "South Farm",
                    Boundary = new Polygon(new LinearRing(new[] { new Coordinate(4,4), new Coordinate(4,5), new Coordinate(5,5), new Coordinate(5,4), new Coordinate(4,4) })),
                    FieldType = FieldType.Orchard,
                    CurrentCropId = 3,
                    SoilId = 2,
                    SowingDate = new DateTime(2024, 4, 15,0,0,0,DateTimeKind.Utc)
                }
            };
            dbContext.Fields.AddRange(fields);
            await dbContext.SaveChangesAsync();
        }

        // Seed FieldCropHistories
        if (!dbContext.FieldCropHistories.Any())
        {
            var histories = new List<FieldCropHistory>
            {
                new FieldCropHistory {
                    FieldId = 1,
                    CropId = 1,
                    PlantedDate = new DateOnly(2023, 9, 20),
                    HarvestedDate = new DateOnly(2024, 7, 10),
                    Yield = 6.2,
                    Notes = "Good season."
                },
                new FieldCropHistory {
                    FieldId = 2,
                    CropId = 2,
                    PlantedDate = new DateOnly(2023, 5, 5),
                    HarvestedDate = new DateOnly(2023, 9, 1),
                    Yield = 7.8,
                    Notes = "Average yield."
                }
            };
            dbContext.FieldCropHistories.AddRange(histories);
            await dbContext.SaveChangesAsync();
        }
    }
}
