using NetTopologySuite.Geometries;
using SmartAgroPlan.BLL.Enums;
using SmartAgroPlan.BLL.Services.Recommendations;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Services.Recommendations
{
    public class RecommendationServiceTests
    {
        private readonly RecommendationService _service = new();
        private readonly Field _testField;
        private readonly CropVariety _testCropVariety;

        public RecommendationServiceTests()
        {
            // Create test polygon
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

            _testField = new Field
            {
                Id = 1,
                Name = "Test Field",
                Location = "Test Location",
                Boundary = polygon,
                FieldType = FieldType.Arable,
                CurrentCropId = 1,
                SoilId = 1
            };

            _testCropVariety = new CropVariety
            {
                Id = 1,
                Name = "Test Crop",
                CropType = CropType.Wheat,
                WaterRequirement = 500.0, // mm
                FertilizerRequirement = 120.0, // kg/ha
                GrowingDuration = 120, // days
                SowingStart = new DayMonth(15, 9),
                SowingEnd = new DayMonth(30, 9),
                MinTemperature = 5.0,
                MaxTemperature = 30.0,
                HarvestYield = 6.5,
                OptimalSoilId = 1,
                AdditionalNotes = "Test crop variety"
            };
        }

        [Fact]
        public void GenerateWeekly_GerminationStage_ReturnsCorrectRecommendation()
        {
            // Arrange
            var stage = GrowthStage.Germination;
            var currentDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, _testCropVariety, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testField.Id, result.FieldId);
            Assert.Equal(_testCropVariety.Name, result.Crop);
            Assert.Equal(stage.ToString(), result.GrowthStage);

            // Water calculation: 500 * 0.1 = 50mm for stage, ~12 days (120 * 0.1), ~1.7 weeks, so ~50mm/week
            Assert.True(result.WaterRecommendationMm > 40 && result.WaterRecommendationMm < 60);

            // Fertilizer calculation: 120 * 0.2 = 24kg/ha for stage, ~14.12kg/ha per week
            Assert.True(result.FertilizerRecommendationKgHa > 10 && result.FertilizerRecommendationKgHa < 30);

            Assert.Contains("Germination", result.Notes);
        }

        [Fact]
        public void GenerateWeekly_VegetativeStage_ReturnsCorrectRecommendation()
        {
            // Arrange
            var stage = GrowthStage.Vegetative;
            var currentDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, _testCropVariety, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testField.Id, result.FieldId);
            Assert.Equal(_testCropVariety.Name, result.Crop);
            Assert.Equal(stage.ToString(), result.GrowthStage);

            // Water calculation: 500 * 0.3 = 150mm for stage, ~36 days (120 * 0.3), ~5.1 weeks, so ~29.41mm/week
            Assert.True(result.WaterRecommendationMm > 25 && result.WaterRecommendationMm < 35);

            // Fertilizer calculation: 120 * 0.4 = 48kg/ha for stage, ~9.41kg/ha per week
            Assert.True(result.FertilizerRecommendationKgHa > 5 && result.FertilizerRecommendationKgHa < 15);

            Assert.Contains("Vegetative", result.Notes);
        }

        [Fact]
        public void GenerateWeekly_FloweringStage_ReturnsCorrectRecommendation()
        {
            // Arrange
            var stage = GrowthStage.Flowering;
            var currentDate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, _testCropVariety, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(stage.ToString(), result.GrowthStage);

            // Water calculation: 500 * 0.25 = 125mm for stage
            Assert.True(result.WaterRecommendationMm > 0);

            // Fertilizer calculation: 120 * 0.3 = 36kg/ha for stage
            Assert.True(result.FertilizerRecommendationKgHa > 0);

            Assert.Contains("Flowering", result.Notes);
        }

        [Fact]
        public void GenerateWeekly_GrainFillingStage_ReturnsCorrectRecommendation()
        {
            // Arrange
            var stage = GrowthStage.GrainFilling;
            var currentDate = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, _testCropVariety, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(stage.ToString(), result.GrowthStage);

            // Water calculation: 500 * 0.25 = 125mm for stage
            Assert.True(result.WaterRecommendationMm > 0);

            // Fertilizer calculation: 120 * 0.1 = 12kg/ha for stage
            Assert.True(result.FertilizerRecommendationKgHa >= 0);

            Assert.Contains("GrainFilling", result.Notes);
        }

        [Fact]
        public void GenerateWeekly_MaturityStage_ReturnsCorrectRecommendation()
        {
            // Arrange
            var stage = GrowthStage.Maturity;
            var currentDate = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, _testCropVariety, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(stage.ToString(), result.GrowthStage);

            // Water calculation: 500 * 0.1 = 50mm for stage
            Assert.True(result.WaterRecommendationMm > 0);

            // Fertilizer calculation: no fertilizer for maturity stage (0)
            Assert.Equal(0, result.FertilizerRecommendationKgHa);

            Assert.Contains("Maturity", result.Notes);
        }

        [Fact]
        public void GenerateWeekly_SowingStage_ReturnsNoRecommendation()
        {
            // Arrange
            var stage = GrowthStage.Sowing;
            var currentDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, _testCropVariety, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testField.Id, result.FieldId);
            Assert.Equal(_testCropVariety.Name, result.Crop);
            Assert.Equal(stage.ToString(), result.GrowthStage);
            Assert.Equal(0, result.WaterRecommendationMm);
            Assert.Equal(0, result.FertilizerRecommendationKgHa);
            Assert.Equal("No recommendations for this stage.", result.Notes);
        }

        [Fact]
        public void GenerateWeekly_HarvestStage_ReturnsNoRecommendation()
        {
            // Arrange
            var stage = GrowthStage.Harvest;
            var currentDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, _testCropVariety, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testField.Id, result.FieldId);
            Assert.Equal(_testCropVariety.Name, result.Crop);
            Assert.Equal(stage.ToString(), result.GrowthStage);
            Assert.Equal(0, result.WaterRecommendationMm);
            Assert.Equal(0, result.FertilizerRecommendationKgHa);
            Assert.Equal("No recommendations for this stage.", result.Notes);
        }

        [Fact]
        public void GenerateWeekly_NullCropName_HandlesGracefully()
        {
            // Arrange
            var cropWithNullName = new CropVariety
            {
                Id = 1,
                Name = null, // Null name
                CropType = CropType.Wheat,
                WaterRequirement = 500.0,
                FertilizerRequirement = 120.0,
                GrowingDuration = 120,
                SowingStart = new DayMonth(15, 9),
                SowingEnd = new DayMonth(30, 9),
                MinTemperature = 5.0,
                MaxTemperature = 30.0,
                HarvestYield = 6.5,
                OptimalSoilId = 1
            };

            var stage = GrowthStage.Vegetative;
            var currentDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, cropWithNullName, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Unknown", result.Crop);
        }

        [Theory]
        [InlineData(GrowthStage.Germination)]
        [InlineData(GrowthStage.Vegetative)]
        [InlineData(GrowthStage.Flowering)]
        [InlineData(GrowthStage.GrainFilling)]
        [InlineData(GrowthStage.Maturity)]
        public void GenerateWeekly_ValidStages_ReturnsPositiveValues(GrowthStage stage)
        {
            // Arrange
            var currentDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, _testCropVariety, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WaterRecommendationMm >= 0);
            Assert.True(result.FertilizerRecommendationKgHa >= 0);
            Assert.NotEmpty(result.Notes);
            Assert.Contains("Weekly recommendation", result.Notes);
            Assert.Contains("days", result.Notes);
            Assert.Contains("Adjust for soil and weather", result.Notes);
        }

        [Fact]
        public void GenerateWeekly_ZeroWaterRequirement_ReturnsZeroWaterRecommendation()
        {
            // Arrange
            var cropWithZeroWater = new CropVariety
            {
                Id = 1,
                Name = "Test Crop",
                CropType = CropType.Wheat,
                WaterRequirement = 0.0, // Zero water requirement
                FertilizerRequirement = 120.0,
                GrowingDuration = 120,
                SowingStart = new DayMonth(15, 9),
                SowingEnd = new DayMonth(30, 9),
                MinTemperature = 5.0,
                MaxTemperature = 30.0,
                HarvestYield = 6.5,
                OptimalSoilId = 1
            };

            var stage = GrowthStage.Vegetative;
            var currentDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, cropWithZeroWater, stage, currentDate);

            // Assert
            Assert.Equal(0, result.WaterRecommendationMm);
            Assert.True(result.FertilizerRecommendationKgHa > 0); // Should still have fertilizer
        }

        [Fact]
        public void GenerateWeekly_ZeroFertilizerRequirement_ReturnsZeroFertilizerRecommendation()
        {
            // Arrange
            var cropWithZeroFertilizer = new CropVariety
            {
                Id = 1,
                Name = "Test Crop",
                CropType = CropType.Wheat,
                WaterRequirement = 500.0,
                FertilizerRequirement = 0.0, // Zero fertilizer requirement
                GrowingDuration = 120,
                SowingStart = new DayMonth(15, 9),
                SowingEnd = new DayMonth(30, 9),
                MinTemperature = 5.0,
                MaxTemperature = 30.0,
                HarvestYield = 6.5,
                OptimalSoilId = 1
            };

            var stage = GrowthStage.Vegetative;
            var currentDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, cropWithZeroFertilizer, stage, currentDate);

            // Assert
            Assert.Equal(0, result.FertilizerRecommendationKgHa);
            Assert.True(result.WaterRecommendationMm > 0); // Should still have water
        }

        [Fact]
        public void GenerateWeekly_ShortGrowingDuration_HandlesMiniumumWeeks()
        {
            // Arrange
            var cropWithShortDuration = new CropVariety
            {
                Id = 1,
                Name = "Fast Crop",
                CropType = CropType.Wheat,
                WaterRequirement = 100.0,
                FertilizerRequirement = 50.0,
                GrowingDuration = 7, // Very short duration - only 1 week total
                SowingStart = new DayMonth(15, 9),
                SowingEnd = new DayMonth(30, 9),
                MinTemperature = 5.0,
                MaxTemperature = 30.0,
                HarvestYield = 6.5,
                OptimalSoilId = 1
            };

            var stage = GrowthStage.Vegetative;
            var currentDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, cropWithShortDuration, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            // With minimum 1 week enforced, should still get valid recommendations
            Assert.True(result.WaterRecommendationMm >= 0);
            Assert.True(result.FertilizerRecommendationKgHa >= 0);
        }

        [Fact]
        public void GenerateWeekly_ReturnsRoundedValues()
        {
            // Arrange
            var stage = GrowthStage.Germination;
            var currentDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = _service.GenerateWeekly(_testField, _testCropVariety, stage, currentDate);

            // Assert
            Assert.NotNull(result);
            // Check that values are rounded to 2 decimal places
            Assert.Equal(Math.Round(result.WaterRecommendationMm, 2), result.WaterRecommendationMm);
            Assert.Equal(Math.Round(result.FertilizerRecommendationKgHa, 2), result.FertilizerRecommendationKgHa);
        }
    }
}