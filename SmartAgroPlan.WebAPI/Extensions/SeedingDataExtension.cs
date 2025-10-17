using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
                new()
                {
                    Type = SoilType.Clay,
                    WaterRetention = 55.0,
                    Acidity = 7,
                    NutrientContent = 1500,
                    OrganicMatter = 4.0,
                    SoilDensity = 1.9,
                    ErosionRisk = 15.0
                },
                new()
                {
                    Type = SoilType.Sandy,
                    WaterRetention = 10.0,
                    Acidity = 6.0,
                    NutrientContent = 800,
                    OrganicMatter = 0.5,
                    SoilDensity = 1.6,
                    ErosionRisk = 70.0
                },
                new()
                {
                    Type = SoilType.Loamy,
                    WaterRetention = 35.0,
                    Acidity = 6.5,
                    NutrientContent = 1200,
                    OrganicMatter = 5.0,
                    SoilDensity = 1.35,
                    ErosionRisk = 30.0
                },
                new()
                {
                    Type = SoilType.Peaty,
                    WaterRetention = 75.0,
                    Acidity = 5,
                    NutrientContent = 800.0,
                    OrganicMatter = 30.0,
                    SoilDensity = 1.0,
                    ErosionRisk = 80
                },
                new()
                {
                    Type = SoilType.Saline,
                    WaterRetention = 25.0,
                    Acidity = 8.0,
                    NutrientContent = 600.0,
                    OrganicMatter = 1.5,
                    SoilDensity = 1.2,
                    ErosionRisk = 45.0
                },
                new()
                {
                    Type = SoilType.Chalky,
                    WaterRetention = 20.0,
                    Acidity = 8.5,
                    NutrientContent = 700.0,
                    OrganicMatter = 1.0,
                    SoilDensity = 1.4,
                    ErosionRisk = 50.0
                },
                new()
                {
                    Type = SoilType.Silty,
                    WaterRetention = 40.0,
                    Acidity = 6.8,
                    NutrientContent = 1300.0,
                    OrganicMatter = 3.0,
                    SoilDensity = 1.3,
                    ErosionRisk = 60.0
                },
                new()
                {
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
            List<CropVariety> crops;
            // Attempt to load from CSV
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(name =>
                    name.EndsWith("crop_varieties_dataset.csv", StringComparison.OrdinalIgnoreCase));

            using (var stream = assembly.GetManifestResourceStream(resourceName!))
            {
                if (stream == null)
                {
                    // Fallback to default crops if embedded resource not found
                    var soilLoamy = dbContext.Soils.First(s => s.Type == SoilType.Loamy);
                    var soilClay = dbContext.Soils.First(s => s.Type == SoilType.Clay);
                    var soilSandy = dbContext.Soils.First(s => s.Type == SoilType.Sandy);
                    crops = new List<CropVariety>
                    {
                        new()
                        {
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
                        new()
                        {
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
                        new()
                        {
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
                else
                {
                    using var reader = new StreamReader(stream);
                    var csvContent = await reader.ReadToEndAsync();
                    var soils = dbContext.Soils.ToList();
                    crops = CropVarietyCsvParser.Parse(csvContent, soils);
                }
            }

            if (crops.Count != 0)
            {
                dbContext.Crops.AddRange(crops);
                await dbContext.SaveChangesAsync();
            }
        }

        // Seed Fields
        if (!dbContext.Fields.Any())
        {
            var fields = new List<Field>
            {
                new()
                {
                    Name = "Сад під Черешнею",
                    Location = "Україна, с. Реклинець",
                    Boundary = new Polygon(new LinearRing([
                        new Coordinate(24.225911, 50.217076),
                        new Coordinate(24.228916, 50.21698),
                        new Coordinate(24.231363, 50.216651),
                        new Coordinate(24.234884, 50.21698),
                        new Coordinate(24.235356, 50.218106),
                        new Coordinate(24.23381, 50.218957),
                        new Coordinate(24.231471, 50.218751),
                        new Coordinate(24.229002, 50.219493),
                        new Coordinate(24.227091, 50.219918),
                        new Coordinate(24.225911, 50.217076)
                    ])),
                    FieldType = FieldType.Arable,
                    CurrentCropId = 2,
                    SoilId = 3
                },
                new()
                {
                    Name = "За лугом",
                    Location = "Реклинець",
                    Boundary = new Polygon(new LinearRing([
                        new Coordinate(24.193461, 50.222225),
                        new Coordinate(24.193955, 50.221291),
                        new Coordinate(24.19636, 50.221127),
                        new Coordinate(24.197731, 50.22033),
                        new Coordinate(24.199022, 50.219809),
                        new Coordinate(24.201855, 50.2186),
                        new Coordinate(24.204002, 50.216623),
                        new Coordinate(24.20522, 50.216328),
                        new Coordinate(24.206125, 50.216431),
                        new Coordinate(24.208249, 50.216239),
                        new Coordinate(24.211169, 50.216651),
                        new Coordinate(24.212671, 50.217255),
                        new Coordinate(24.212714, 50.218106),
                        new Coordinate(24.21111, 50.218628),
                        new Coordinate(24.209451, 50.220275),
                        new Coordinate(24.209709, 50.221209),
                        new Coordinate(24.212585, 50.220879),
                        new Coordinate(24.212642, 50.222499),
                        new Coordinate(24.212371, 50.223763),
                        new Coordinate(24.208721, 50.223735),
                        new Coordinate(24.208192, 50.223756),
                        new Coordinate(24.208589, 50.223914),
                        new Coordinate(24.213014, 50.224078),
                        new Coordinate(24.212648, 50.225122),
                        new Coordinate(24.203933, 50.226865),
                        new Coordinate(24.201765, 50.224051),
                        new Coordinate(24.200488, 50.223852),
                        new Coordinate(24.199167, 50.223859),
                        new Coordinate(24.196548, 50.223941),
                        new Coordinate(24.196551, 50.22438),
                        new Coordinate(24.196578, 50.224662),
                        new Coordinate(24.196331, 50.22483),
                        new Coordinate(24.19481, 50.224888),
                        new Coordinate(24.194616, 50.224614),
                        new Coordinate(24.194638, 50.22309),
                        new Coordinate(24.1932, 50.223145),
                        new Coordinate(24.193461, 50.222225)
                    ])),
                    FieldType = FieldType.Arable,
                    CurrentCropId = 49,
                    SoilId = 1
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
                new()
                {
                    FieldId = 1,
                    CropId = 1,
                    PlantedDate = new DateOnly(2022, 9, 20),
                    HarvestedDate = new DateOnly(2022, 7, 10),
                    Yield = 6.2,
                    Notes = "Good season."
                },
                new()
                {
                    FieldId = 1,
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

        // Seed FieldConditions
        if (!dbContext.FieldConditions.Any())
        {
            var conditions = new List<FieldCondition>
            {
                new()
                {
                    FieldId = 1,
                    RecordedAt = DateTime.UtcNow.AddDays(-10),
                    SoilMoisture = 0.305,
                    SoilPh = 6.8,
                    Nitrogen = 100,
                    Phosphorus = 50,
                    Potassium = 70,
                    Temperature = 22.5,
                    Rainfall = 15.0,
                    Notes = "Normal conditions."
                },
                new()
                {
                    FieldId = 1,
                    RecordedAt = DateTime.UtcNow.AddDays(-5),
                    SoilMoisture = 0.28,
                    SoilPh = 6.7,
                    Nitrogen = 95,
                    Phosphorus = 48,
                    Potassium = 68,
                    Temperature = 24.0,
                    Rainfall = 5.0,
                    Notes = "Slightly dry."
                },
                new()
                {
                    FieldId = 2,
                    RecordedAt = DateTime.UtcNow.AddDays(-7),
                    SoilMoisture = 0.35,
                    SoilPh = 7.0,
                    Nitrogen = 110,
                    Phosphorus = 55,
                    Potassium = 75,
                    Temperature = 20.0,
                    Rainfall = 20.0,
                    Notes = "Good moisture levels."
                }
            };
            dbContext.FieldConditions.AddRange(conditions);
            await dbContext.SaveChangesAsync();
        }

        // Seed CropCoefficientDefinitions
        if (!dbContext.CropCoefficientDefinitions.Any())
        {
            var definitions = new List<CropCoefficientDefinition>
            {
                new()
                {
                    CropType = CropType.Wheat,
                    KcIni = 0.4,
                    KcMid = 1.15,
                    KcEnd = 0.25,
                    LIni = 30,
                    LDev = 140,
                    LMid = 40,
                    LLate = 30
                },
                new()
                {
                    CropType = CropType.Corn,
                    KcIni = 0.3,
                    KcMid = 1.2,
                    KcEnd = 0.35,
                    LIni = 30,
                    LDev = 40,
                    LMid = 50,
                    LLate = 40
                },
                new()
                {
                    CropType = CropType.Sunflower,
                    KcIni = 0.35,
                    KcMid = 1.0,
                    KcEnd = 0.35,
                    LIni = 25,
                    LDev = 35,
                    LMid = 45,
                    LLate = 25
                },
                new()
                {
                    CropType = CropType.Soy,
                    KcIni = 0.4,
                    KcMid = 1.15,
                    KcEnd = 0.5,
                    LIni = 20,
                    LDev = 35,
                    LMid = 60,
                    LLate = 25
                },
                new()
                {
                    CropType = CropType.Potato,
                    KcIni = 0.5,
                    KcMid = 1.15,
                    KcEnd = 0.75,
                    LIni = 30,
                    LDev = 35,
                    LMid = 50,
                    LLate = 30
                },
                new()
                {
                    CropType = CropType.Rapeseed,
                    KcIni = 0.35,
                    KcMid = 1.05,
                    KcEnd = 0.35,
                    LIni = 20,
                    LDev = 40,
                    LMid = 60,
                    LLate = 30
                },
                new()
                {
                    CropType = CropType.SugarBeet,
                    KcIni = 0.35,
                    KcMid = 1.2,
                    KcEnd = 0.7,
                    LIni = 30,
                    LDev = 40,
                    LMid = 50,
                    LLate = 50
                },
                new()
                {
                    CropType = CropType.Barley,
                    KcIni = 0.3,
                    KcMid = 1.15,
                    KcEnd = 0.25,
                    LIni = 20,
                    LDev = 110,
                    LMid = 40,
                    LLate = 30
                },
                new()
                {
                    CropType = CropType.Oats,
                    KcIni = 0.3,
                    KcMid = 1.15,
                    KcEnd = 0.25,
                    LIni = 40,
                    LDev = 30,
                    LMid = 40,
                    LLate = 20
                },
                new()
                {
                    CropType = CropType.Rye,
                    KcIni = 1.05,
                    KcMid = 1.2,
                    KcEnd = 0.9,
                    LIni = 30,
                    LDev = 30,
                    LMid = 60,
                    LLate = 30
                },
                new()
                {
                    CropType = CropType.Tomato,
                    KcIni = 0.6,
                    KcMid = 1.15,
                    KcEnd = 0.9,
                    LIni = 30,
                    LDev = 40,
                    LMid = 45,
                    LLate = 30
                },
                new()
                {
                    CropType = CropType.Another,
                    KcIni = 0.5,
                    KcMid = 1.0,
                    KcEnd = 0.5,
                    LIni = 30,
                    LDev = 50,
                    LMid = 50,
                    LLate = 30
                }
            };
            dbContext.CropCoefficientDefinitions.AddRange(definitions);
            await dbContext.SaveChangesAsync();
        }
    }
}
