using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Persistence;

namespace SmartAgroPlan.XUnitTest.DAL_Tests.Utils;

public static class InMemoryDbContextFactory
{
    public static SmartAgroPlanDbContext CreateInMemoryDbContext(string databaseName = "")
    {
        if (string.IsNullOrEmpty(databaseName)) databaseName = Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder<SmartAgroPlanDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        var context = new SmartAgroPlanDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public static SmartAgroPlanDbContext CreateSeededInMemoryDbContext(string databaseName = "")
    {
        var context = CreateInMemoryDbContext(databaseName);
        SeedTestData(context);
        return context;
    }

    public static void SeedTestData(SmartAgroPlanDbContext context)
    {
        // Clear existing data
        context.FieldCropHistories.RemoveRange(context.FieldCropHistories);
        context.Fields.RemoveRange(context.Fields);
        context.Crops.RemoveRange(context.Crops);
        context.Soils.RemoveRange(context.Soils);
        context.SaveChanges();

        // Seed Soils
        var soils = new List<Soil>
        {
            new()
            {
                Id = 1,
                Type = SoilType.Loamy,
                WaterRetention = 75.5,
                Acidity = 6.5,
                NutrientContent = 850.0,
                OrganicMatter = 3.2,
                SoilDensity = 1.4,
                ErosionRisk = 15.0
            },
            new()
            {
                Id = 2,
                Type = SoilType.Clay,
                WaterRetention = 85.0,
                Acidity = 7.2,
                NutrientContent = 920.0,
                OrganicMatter = 4.1,
                SoilDensity = 1.6,
                ErosionRisk = 25.0
            },
            new()
            {
                Id = 3,
                Type = SoilType.Sandy,
                WaterRetention = 45.0,
                Acidity = 6.0,
                NutrientContent = 650.0,
                OrganicMatter = 2.0,
                SoilDensity = 1.2,
                ErosionRisk = 35.0
            }
        };

        context.Soils.AddRange(soils);
        context.SaveChanges();

        // Seed CropVarieties
        var cropVarieties = new List<CropVariety>
        {
            new()
            {
                Id = 1,
                Name = "Winter Wheat",
                CropType = CropType.Wheat,
                WaterRequirement = 450.0,
                FertilizerRequirement = 120.0,
                GrowingDuration = 220,
                LIni = 30,
                LDev = 55,
                LMid = 90,
                LLate = 45,
                SowingStart = new DayMonth(15, 9),
                SowingEnd = new DayMonth(30, 10),
                MinTemperature = -5.0,
                MaxTemperature = 30.0,
                HarvestYield = 6.5,
                OptimalSoilId = 1,
                AdditionalNotes = "High-yield winter wheat variety"
            },
            new()
            {
                Id = 2,
                Name = "Sweet Corn",
                CropType = CropType.Corn,
                WaterRequirement = 600.0,
                FertilizerRequirement = 180.0,
                GrowingDuration = 110,
                LIni = 20,
                LDev = 30,
                LMid = 40,
                LLate = 20,
                SowingStart = new DayMonth(1, 5),
                SowingEnd = new DayMonth(15, 6),
                MinTemperature = 10.0,
                MaxTemperature = 35.0,
                HarvestYield = 8.2,
                OptimalSoilId = 2,
                AdditionalNotes = "Sweet corn for direct consumption"
            },
            new()
            {
                Id = 3,
                Name = "Sunflower",
                CropType = CropType.Sunflower,
                WaterRequirement = 400.0,
                FertilizerRequirement = 90.0,
                GrowingDuration = 120,
                LIni = 20,
                LDev = 30,
                LMid = 50,
                LLate = 20,
                SowingStart = new DayMonth(15, 4),
                SowingEnd = new DayMonth(30, 5),
                MinTemperature = 8.0,
                MaxTemperature = 32.0,
                HarvestYield = 2.8,
                OptimalSoilId = 3,
                AdditionalNotes = "Oil sunflower variety"
            }
        };

        context.Crops.AddRange(cropVarieties);
        context.SaveChanges();

        // Create sample polygon for fields
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(0, 100),
            new Coordinate(100, 100),
            new Coordinate(100, 0),
            new Coordinate(0, 0)
        };
        var polygon = geometryFactory.CreatePolygon(coordinates);

        // Seed Fields
        var fields = new List<Field>
        {
            new()
            {
                Id = 1,
                Name = "North Field",
                Location = "Northern section of the farm",
                Boundary = polygon,
                FieldType = FieldType.Arable,
                CurrentCropId = 1,
                SoilId = 1
            },
            new()
            {
                Id = 2,
                Name = "South Field",
                Location = "Southern section of the farm",
                Boundary = polygon,
                FieldType = FieldType.Arable,
                CurrentCropId = 2,
                SoilId = 2
            },
            new()
            {
                Id = 3,
                Name = "East Field",
                Location = "Eastern section of the farm",
                Boundary = polygon,
                FieldType = FieldType.Pasture,
                CurrentCropId = 3,
                SoilId = 3
            }
        };

        context.Fields.AddRange(fields);
        context.SaveChanges();

        // Seed FieldCropHistory
        var fieldCropHistories = new List<FieldCropHistory>
        {
            new()
            {
                Id = 1,
                FieldId = 1,
                CropId = 1,
                PlantedDate = new DateOnly(2023, 9, 20),
                HarvestedDate = new DateOnly(2024, 6, 15),
                Yield = 6.2,
                Notes = "Good harvest, weather was favorable"
            },
            new()
            {
                Id = 2,
                FieldId = 2,
                CropId = 2,
                PlantedDate = new DateOnly(2024, 5, 10),
                HarvestedDate = new DateOnly(2024, 8, 25),
                Yield = 7.8,
                Notes = "Excellent yield due to optimal conditions"
            },
            new()
            {
                Id = 3,
                FieldId = 1,
                CropId = 3,
                PlantedDate = new DateOnly(2024, 4, 20),
                HarvestedDate = null,
                Yield = null,
                Notes = "Currently growing"
            }
        };

        context.FieldCropHistories.AddRange(fieldCropHistories);
        context.SaveChanges();

        // Ensure all entities are detached for clean testing
        context.ChangeTracker.Clear();
    }

    public static List<Soil> GetTestSoils()
    {
        return new List<Soil>
        {
            new()
            {
                Type = SoilType.Loamy,
                WaterRetention = 75.5,
                Acidity = 6.5,
                NutrientContent = 850.0,
                OrganicMatter = 3.2,
                SoilDensity = 1.4,
                ErosionRisk = 15.0
            },
            new()
            {
                Type = SoilType.Clay,
                WaterRetention = 85.0,
                Acidity = 7.2,
                NutrientContent = 920.0,
                OrganicMatter = 4.1,
                SoilDensity = 1.6,
                ErosionRisk = 25.0
            }
        };
    }

    public static Soil GetTestSoil()
    {
        return new Soil
        {
            Type = SoilType.Loamy,
            WaterRetention = 75.5,
            Acidity = 6.5,
            NutrientContent = 850.0,
            OrganicMatter = 3.2,
            SoilDensity = 1.4,
            ErosionRisk = 15.0
        };
    }
}
