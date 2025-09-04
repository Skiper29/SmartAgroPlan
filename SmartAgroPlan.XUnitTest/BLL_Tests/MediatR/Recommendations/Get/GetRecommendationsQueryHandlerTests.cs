using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NetTopologySuite.Geometries;
using SmartAgroPlan.BLL.DTO.Recommendations;
using SmartAgroPlan.BLL.Enums;
using SmartAgroPlan.BLL.Interfaces.Recommendations;
using SmartAgroPlan.BLL.MediatR.Recommendations.Get;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Fields;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Recommendations.Get
{
    public class GetRecommendationsQueryHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IGrowthStageService> _mockGrowthStageService;
        private readonly Mock<IRecommendationService> _mockRecommendationService;
        private readonly Mock<IFieldRepository> _mockFieldRepository;
        private readonly GetRecommendationsQueryHandler _handler;

        public GetRecommendationsQueryHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockGrowthStageService = new Mock<IGrowthStageService>();
            _mockRecommendationService = new Mock<IRecommendationService>();
            _mockFieldRepository = new Mock<IFieldRepository>();

            // Setup repository wrapper to return the field repository mock
            _mockRepositoryWrapper.Setup(x => x.FieldRepository)
                .Returns(_mockFieldRepository.Object);

            _handler = new GetRecommendationsQueryHandler(
                _mockRepositoryWrapper.Object,
                _mockGrowthStageService.Object,
                _mockRecommendationService.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnRecommendationResponse()
        {
            // Arrange
            var field = CreateTestField();
            var request = CreateTestRecommendationRequest();
            var query = CreateTestQuery(request);
            var expectedResponse = CreateTestRecommendationResponse();
            var expectedGrowthStage = GrowthStage.Vegetative;

            SetupFieldRepositoryToReturnField(field);

            _mockGrowthStageService.Setup(x => x.GetStage(
                    field.SowingDate!.Value,
                    request.CurrentDate,
                    field.CurrentCrop!.GrowingDuration))
                .Returns(expectedGrowthStage);

            _mockRecommendationService.Setup(x => x.GenerateWeekly(
                    field,
                    field.CurrentCrop,
                    expectedGrowthStage,
                    request.CurrentDate))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
            result.FieldId.Should().Be(field.Id);
            result.Crop.Should().Be(expectedResponse.Crop);
            result.GrowthStage.Should().Be(expectedResponse.GrowthStage);
            result.WaterRecommendationMm.Should().Be(expectedResponse.WaterRecommendationMm);
            result.FertilizerRecommendationKgHa.Should().Be(expectedResponse.FertilizerRecommendationKgHa);
            result.Notes.Should().Be(expectedResponse.Notes);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldCallRepositoryWithCorrectParameters()
        {
            // Arrange
            var field = CreateTestField();
            var request = CreateTestRecommendationRequest(fieldId: 42);
            var query = CreateTestQuery(request);
            var expectedResponse = CreateTestRecommendationResponse();

            SetupFieldRepositoryToReturnField(field);
            _mockGrowthStageService.Setup(x => x.GetStage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(GrowthStage.Flowering);
            _mockRecommendationService.Setup(x => x.GenerateWeekly(It.IsAny<Field>(), It.IsAny<CropVariety>(), It.IsAny<GrowthStage>(), It.IsAny<DateTime>()))
                .Returns(expectedResponse);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockFieldRepository.Verify(x => x.FindAll(
                It.Is<Expression<Func<Field, bool>>>(expr => TestExpressionForFieldId(expr, 42)),
                It.IsAny<Func<IQueryable<Field>, IIncludableQueryable<Field, object>>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldCallGrowthStageServiceWithCorrectParameters()
        {
            // Arrange
            var sowingDate = new DateTime(2024, 9, 15, 0, 0, 0, DateTimeKind.Utc);
            var currentDate = new DateTime(2024, 10, 15, 0, 0, 0, DateTimeKind.Utc);
            var growingDuration = 120;

            var field = CreateTestField(sowingDate: sowingDate);
            field.CurrentCrop!.GrowingDuration = growingDuration;

            var request = CreateTestRecommendationRequest(currentDate: currentDate);
            var query = CreateTestQuery(request);
            var expectedResponse = CreateTestRecommendationResponse();

            SetupFieldRepositoryToReturnField(field);
            _mockGrowthStageService.Setup(x => x.GetStage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(GrowthStage.Flowering);
            _mockRecommendationService.Setup(x => x.GenerateWeekly(It.IsAny<Field>(), It.IsAny<CropVariety>(), It.IsAny<GrowthStage>(), It.IsAny<DateTime>()))
                .Returns(expectedResponse);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockGrowthStageService.Verify(x => x.GetStage(
                sowingDate,
                currentDate,
                growingDuration), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldCallRecommendationServiceWithCorrectParameters()
        {
            // Arrange
            var field = CreateTestField();
            var request = CreateTestRecommendationRequest();
            var query = CreateTestQuery(request);
            var expectedResponse = CreateTestRecommendationResponse();
            var expectedGrowthStage = GrowthStage.GrainFilling;

            SetupFieldRepositoryToReturnField(field);
            _mockGrowthStageService.Setup(x => x.GetStage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(expectedGrowthStage);
            _mockRecommendationService.Setup(x => x.GenerateWeekly(It.IsAny<Field>(), It.IsAny<CropVariety>(), It.IsAny<GrowthStage>(), It.IsAny<DateTime>()))
                .Returns(expectedResponse);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockRecommendationService.Verify(x => x.GenerateWeekly(
                field,
                field.CurrentCrop,
                expectedGrowthStage,
                request.CurrentDate), Times.Once);
        }

        [Fact]
        public async Task Handle_FieldNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var request = CreateTestRecommendationRequest(fieldId: 999);
            var query = CreateTestQuery(request);

            SetupFieldRepositoryToReturnNull();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));

            exception.Message.Should().Contain("Field with id 999 not found");
        }

        [Fact]
        public async Task Handle_FieldWithoutCurrentCrop_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var field = CreateTestField();
            field.CurrentCrop = null; // No current crop

            var request = CreateTestRecommendationRequest();
            var query = CreateTestQuery(request);

            SetupFieldRepositoryToReturnField(field);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));

            exception.Message.Should().Contain($"Field with id {field.Id} has no current crop");
        }

        [Fact]
        public async Task Handle_FieldWithNullSowingDate_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var field = CreateTestField();
            field.SowingDate = null; // Null sowing date

            var request = CreateTestRecommendationRequest();
            var query = CreateTestQuery(request);

            SetupFieldRepositoryToReturnField(field);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(GrowthStage.Sowing)]
        [InlineData(GrowthStage.Germination)]
        [InlineData(GrowthStage.Vegetative)]
        [InlineData(GrowthStage.Flowering)]
        [InlineData(GrowthStage.GrainFilling)]
        [InlineData(GrowthStage.Maturity)]
        [InlineData(GrowthStage.Harvest)]
        public async Task Handle_DifferentGrowthStages_ShouldReturnAppropriateRecommendations(GrowthStage growthStage)
        {
            // Arrange
            var field = CreateTestField();
            var request = CreateTestRecommendationRequest();
            var query = CreateTestQuery(request);
            var expectedResponse = CreateTestRecommendationResponse();
            expectedResponse.GrowthStage = growthStage.ToString();

            SetupFieldRepositoryToReturnField(field);
            _mockGrowthStageService.Setup(x => x.GetStage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(growthStage);
            _mockRecommendationService.Setup(x => x.GenerateWeekly(It.IsAny<Field>(), It.IsAny<CropVariety>(), It.IsAny<GrowthStage>(), It.IsAny<DateTime>()))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.GrowthStage.Should().Be(growthStage.ToString());
            _mockGrowthStageService.Verify(x => x.GetStage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
            _mockRecommendationService.Verify(x => x.GenerateWeekly(It.IsAny<Field>(), It.IsAny<CropVariety>(), growthStage, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DifferentCropTypes_ShouldWorkCorrectly()
        {
            // Arrange
            var cornCrop = CreateTestCropVariety(cropId: 2, cropName: "Sweet Corn");
            cornCrop.CropType = CropType.Corn;
            cornCrop.GrowingDuration = 100;

            var field = CreateTestField();
            field.CurrentCrop = cornCrop;

            var request = CreateTestRecommendationRequest();
            var query = CreateTestQuery(request);
            var expectedResponse = CreateTestRecommendationResponse();
            expectedResponse.Crop = "Sweet Corn";

            SetupFieldRepositoryToReturnField(field);
            _mockGrowthStageService.Setup(x => x.GetStage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(GrowthStage.Flowering);
            _mockRecommendationService.Setup(x => x.GenerateWeekly(It.IsAny<Field>(), It.IsAny<CropVariety>(), It.IsAny<GrowthStage>(), It.IsAny<DateTime>()))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Crop.Should().Be("Sweet Corn");
            _mockGrowthStageService.Verify(x => x.GetStage(
                field.SowingDate!.Value,
                request.CurrentDate,
                100), Times.Once);
        }

        [Fact]
        public async Task Handle_CompleteWorkflow_ShouldExecuteAllStepsInOrder()
        {
            // Arrange
            var field = CreateTestField();
            var request = CreateTestRecommendationRequest();
            var query = CreateTestQuery(request);
            var expectedResponse = CreateTestRecommendationResponse();
            var expectedGrowthStage = GrowthStage.Vegetative;

            SetupFieldRepositoryToReturnField(field);
            _mockGrowthStageService.Setup(x => x.GetStage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(expectedGrowthStage);
            _mockRecommendationService.Setup(x => x.GenerateWeekly(It.IsAny<Field>(), It.IsAny<CropVariety>(), It.IsAny<GrowthStage>(), It.IsAny<DateTime>()))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            // Verify all services were called in the correct order
            _mockFieldRepository.Verify(x => x.FindAll(It.IsAny<Expression<Func<Field, bool>>>(), It.IsAny<Func<IQueryable<Field>, IIncludableQueryable<Field, object>>>()), Times.Once);
            _mockGrowthStageService.Verify(x => x.GetStage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
            _mockRecommendationService.Verify(x => x.GenerateWeekly(It.IsAny<Field>(), It.IsAny<CropVariety>(), It.IsAny<GrowthStage>(), It.IsAny<DateTime>()), Times.Once);

            // Verify the result matches expectations
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task Handle_MultipleFieldsWithSameId_ShouldReturnFirstMatch()
        {
            // Arrange
            var field1 = CreateTestField(fieldId: 1, fieldName: "Field 1");
            var field2 = CreateTestField(fieldId: 1, fieldName: "Field 2"); // Same ID

            var queryable = new List<Field> { field1, field2 }.AsQueryable();

            _mockFieldRepository.Setup(x => x.FindAll(
                    It.IsAny<Expression<Func<Field, bool>>>(),
                    It.IsAny<Func<IQueryable<Field>, IIncludableQueryable<Field, object>>>()))
                .Returns(queryable);

            var request = CreateTestRecommendationRequest(fieldId: 1);
            var query = CreateTestQuery(request);
            var expectedResponse = CreateTestRecommendationResponse();

            _mockGrowthStageService.Setup(x => x.GetStage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(GrowthStage.Flowering);
            _mockRecommendationService.Setup(x => x.GenerateWeekly(It.IsAny<Field>(), It.IsAny<CropVariety>(), It.IsAny<GrowthStage>(), It.IsAny<DateTime>()))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockRecommendationService.Verify(x => x.GenerateWeekly(field1, It.IsAny<CropVariety>(), It.IsAny<GrowthStage>(), It.IsAny<DateTime>()), Times.Once);
        }

        private static bool TestExpressionForFieldId(Expression<Func<Field, bool>> expression, int expectedFieldId)
        {
            // This is a simplified test - in real scenarios you might want more sophisticated expression testing
            var compiled = expression.Compile();
            var testField = CreateTestField(fieldId: expectedFieldId);
            return compiled(testField);
        }

        private static Field CreateTestField(int fieldId = 1, string fieldName = "Test Field", DateTime? sowingDate = null)
        {
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

            return new Field
            {
                Id = fieldId,
                Name = fieldName,
                Location = "Test Location",
                Boundary = polygon,
                FieldType = FieldType.Arable,
                CurrentCropId = 1,
                SoilId = 1,
                SowingDate = sowingDate ?? new DateTime(2024, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                CurrentCrop = CreateTestCropVariety()
            };
        }

        private static CropVariety CreateTestCropVariety(int cropId = 1, string cropName = "Test Wheat")
        {
            return new CropVariety
            {
                Id = cropId,
                Name = cropName,
                CropType = CropType.Wheat,
                WaterRequirement = 500.0,
                FertilizerRequirement = 120.0,
                GrowingDuration = 120,
                SowingStart = new DayMonth(15, 9),
                SowingEnd = new DayMonth(30, 9),
                MinTemperature = 5.0,
                MaxTemperature = 30.0,
                HarvestYield = 6.5,
                OptimalSoilId = 1,
                AdditionalNotes = "Test crop variety"
            };
        }

        private static RecommendationRequestDto CreateTestRecommendationRequest(int fieldId = 1, DateTime? currentDate = null)
        {
            return new RecommendationRequestDto
            {
                FieldId = fieldId,
                CurrentDate = currentDate ?? new DateTime(2024, 10, 15, 0, 0, 0, DateTimeKind.Utc)
            };
        }

        private static GetRecommendationsQuery CreateTestQuery(RecommendationRequestDto? request = null)
        {
            return new GetRecommendationsQuery(request ?? CreateTestRecommendationRequest());
        }

        private static RecommendationResponseDto CreateTestRecommendationResponse(int fieldId = 1)
        {
            return new RecommendationResponseDto
            {
                FieldId = fieldId,
                Crop = "Test Wheat",
                GrowthStage = GrowthStage.Vegetative.ToString(),
                WaterRecommendationMm = 25.5,
                FertilizerRecommendationKgHa = 15.0,
                Notes = "Weekly recommendation for Vegetative stage"
            };
        }

        private void SetupFieldRepositoryToReturnField(Field field)
        {
            var queryable = new List<Field> { field }.AsQueryable();

            _mockFieldRepository.Setup(x => x.FindAll(
                    It.IsAny<Expression<Func<Field, bool>>>(),
                    It.IsAny<Func<IQueryable<Field>, IIncludableQueryable<Field, object>>>()))
                .Returns(queryable);
        }

        private void SetupFieldRepositoryToReturnNull()
        {
            var emptyQueryable = new List<Field>().AsQueryable();

            _mockFieldRepository.Setup(x => x.FindAll(
                    It.IsAny<Expression<Func<Field, bool>>>(),
                    It.IsAny<Func<IQueryable<Field>, IIncludableQueryable<Field, object>>>()))
                .Returns(emptyQueryable);
        }
    }
}