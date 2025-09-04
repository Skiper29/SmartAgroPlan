using SmartAgroPlan.BLL.Enums;
using SmartAgroPlan.BLL.Services.Recommendations;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Services.Recommendations
{
    public class GrowthStageServiceTests
    {
        private readonly GrowthStageService _service = new();

        [Theory]
        [InlineData(0, GrowthStage.Sowing)]
        [InlineData(1, GrowthStage.Germination)]
        [InlineData(48, GrowthStage.Vegetative)]
        [InlineData(72, GrowthStage.Flowering)]
        [InlineData(102, GrowthStage.GrainFilling)]
        [InlineData(120, GrowthStage.Maturity)]
        [InlineData(121, GrowthStage.Harvest)]
        public void GetStage_ReturnsExpectedStage(int growingDays, GrowthStage expectedStage)
        {
            // Arrange
            var plantingDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentDate = plantingDate.AddDays(growingDays);
            int growingDuration = 120;

            // Act
            var stage = _service.GetStage(plantingDate, currentDate, growingDuration);

            // Assert
            Assert.Equal(expectedStage, stage);
        }

        [Fact]
        public void GetStage_ZeroDuration_ReturnsSowing()
        {
            var plantingDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentDate = plantingDate.AddDays(10);
            int growingDuration = 0;

            var stage = _service.GetStage(plantingDate, currentDate, growingDuration);

            Assert.Equal(GrowthStage.Sowing, stage);
        }

        [Fact]
        public void GetStage_CurrentDateBeforePlanting_ReturnsSowing()
        {
            var plantingDate = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc);
            var currentDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int growingDuration = 120;

            var stage = _service.GetStage(plantingDate, currentDate, growingDuration);

            Assert.Equal(GrowthStage.Sowing, stage);
        }

        [Fact]
        public void GetStage_NegativeGrowingDuration_ReturnsSowing()
        {
            var plantingDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentDate = plantingDate.AddDays(10);
            int growingDuration = -100;

            var stage = _service.GetStage(plantingDate, currentDate, growingDuration);

            Assert.Equal(GrowthStage.Sowing, stage);
        }
    }
}
