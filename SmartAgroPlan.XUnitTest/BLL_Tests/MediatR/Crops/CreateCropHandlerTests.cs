using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.MediatR.Crops.Create;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Crops;

public class CreateCropHandlerTests
{
    private readonly CreateCropHandler _handler;
    private readonly Mock<ILogger<CreateCropHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public CreateCropHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILogger<CreateCropHandler>>();
        _handler = new CreateCropHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenCropCreatedSuccessfully()
    {
        // Arrange
        var (command, cropEntity, cropDto) = SetupSuccessfulCreateScenario();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cropDto);
        VerifyRepositoryInteractions(cropEntity, 1);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappingReturnsNull()
    {
        // Arrange
        var command = CreateCreateCommand();
        SetupMappingToReturnNull();
        const string errorMsg = "Не вдалося відобразити DTO в сутність сорту культури";

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        VerifyNoRepositoryInteractions();
        VerifyLoggerErrorCalled(errorMsg);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        // Arrange
        var (command, cropEntity) = SetupFailingSaveScenario();
        const string errorMsg = "Не вдалося створити сорт культури";

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        VerifyRepositoryInteractions(cropEntity, 0);
        VerifyLoggerErrorCalled(errorMsg);
    }

    private (CreateCropCommand command, CropVariety cropEntity, CropVarietyDto cropDto) SetupSuccessfulCreateScenario()
    {
        var command = CreateCreateCommand();
        var cropEntity = CreateCropVarietyEntity();
        var cropDto = CreateCropVarietyDto();

        _mapperMock.Setup(m => m.Map<CropVariety>(command.CropVariety)).Returns(cropEntity);
        _repositoryWrapperMock.Setup(r => r.CropVarietyRepository.CreateAsync(cropEntity)).ReturnsAsync(cropEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<CropVarietyDto>(cropEntity)).Returns(cropDto);

        return (command, cropEntity, cropDto);
    }

    private (CreateCropCommand command, CropVariety cropEntity) SetupFailingSaveScenario()
    {
        var command = CreateCreateCommand();
        var cropEntity = CreateCropVarietyEntity();

        _mapperMock.Setup(m => m.Map<CropVariety>(command.CropVariety)).Returns(cropEntity);
        _repositoryWrapperMock.Setup(r => r.CropVarietyRepository.CreateAsync(cropEntity)).ReturnsAsync(cropEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        return (command, cropEntity);
    }

    private void SetupMappingToReturnNull()
    {
        _mapperMock.Setup(m => m.Map<CropVariety>(It.IsAny<CropVarietyCreateDto>())).Returns((CropVariety)null!);
    }

    private static CreateCropCommand CreateCreateCommand()
    {
        var createDto = new CropVarietyCreateDto
        {
            Name = "Wheat Variety A",
            CropType = CropType.Wheat,
            FertilizerRequirement = 120,
            WaterRequirement = 500,
            GrowingDuration = 120,
            HarvestYield = 4000,
            MaxTemperature = 25,
            MinTemperature = 10,
            SowingStart = new DayMonth { Day = 15, Month = 4 },
            SowingEnd = new DayMonth { Day = 15, Month = 5 },
            AdditionalNotes = "High yield wheat variety",
            OptimalSoilId = 1
        };
        return new CreateCropCommand(createDto);
    }

    private static CropVariety CreateCropVarietyEntity()
    {
        return new CropVariety
        {
            Id = 1,
            Name = "Wheat Variety A",
            CropType = CropType.Wheat,
            FertilizerRequirement = 120,
            WaterRequirement = 500,
            GrowingDuration = 120,
            HarvestYield = 4000,
            MaxTemperature = 25,
            MinTemperature = 10,
            SowingStart = new DayMonth { Day = 15, Month = 4 },
            SowingEnd = new DayMonth { Day = 15, Month = 5 },
            AdditionalNotes = "High yield wheat variety",
            OptimalSoilId = 1
        };
    }

    private static CropVarietyDto CreateCropVarietyDto()
    {
        return new CropVarietyDto
        {
            Id = 1,
            Name = "Wheat Variety A",
            CropType = CropType.Wheat,
            FertilizerRequirement = 120,
            WaterRequirement = 500,
            GrowingDuration = 120,
            HarvestYield = 4000,
            MaxTemperature = 25,
            MinTemperature = 10,
            SowingStart = new DayMonth { Day = 15, Month = 4 },
            SowingEnd = new DayMonth { Day = 15, Month = 5 },
            AdditionalNotes = "High yield wheat variety",
            OptimalSoil = SoilType.Loamy
        };
    }

    private void VerifyRepositoryInteractions(CropVariety expectedEntity, int expectedSaveResult)
    {
        _repositoryWrapperMock.Verify(r => r.CropVarietyRepository.CreateAsync(expectedEntity), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        if (expectedSaveResult > 0) _mapperMock.Verify(m => m.Map<CropVarietyDto>(expectedEntity), Times.Once);
    }

    private void VerifyNoRepositoryInteractions()
    {
        _repositoryWrapperMock.Verify(r => r.CropVarietyRepository.CreateAsync(It.IsAny<CropVariety>()), Times.Never);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    private void VerifyLoggerErrorCalled(string expectedMessage)
    {
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == expectedMessage),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
