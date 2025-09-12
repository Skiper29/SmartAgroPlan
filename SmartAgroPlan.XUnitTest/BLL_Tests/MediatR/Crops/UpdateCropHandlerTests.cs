using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.MediatR.Crops.Update;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Crops;

public class UpdateCropHandlerTests
{
    private readonly UpdateCropHandler _handler;
    private readonly Mock<ILogger<UpdateCropHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public UpdateCropHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILogger<UpdateCropHandler>>();
        _handler = new UpdateCropHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenCropUpdatedSuccessfully()
    {
        // Arrange
        var (command, cropEntity, _, expectedDto) = SetupSuccessfulUpdateScenario();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDto);
        VerifyRepositoryInteractions(cropEntity, 1);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappingDtoToEntityReturnsNull()
    {
        // Arrange
        var command = CreateUpdateCommand();
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
    public async Task Handle_ShouldReturnFail_WhenOptimalSoilNotFound()
    {
        // Arrange
        var (command, _) = SetupCropEntityMapping();
        SetupSoilRepositoryToReturnNull();
        var errorMsg = $"Не вдалося знайти тип ґрунту з Id = {command.CropVariety.OptimalSoilId}";

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e =>
            e.Message == errorMsg);
        VerifyNoUpdateOrSaveOperations();
        VerifyLoggerErrorCalled(errorMsg);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        // Arrange
        var (command, cropEntity) = SetupFailingSaveScenario();
        const string errorMsg = "Не вдалося оновити сорт культури";

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        VerifyRepositoryInteractions(cropEntity, 0);
        VerifyLoggerErrorCalled(errorMsg);
    }

    [Fact]
    public async Task Handle_ShouldSetOptimalSoilOnEntity_WhenSoilIsFound()
    {
        // Arrange
        var (command, cropEntity, soilEntity, _) = SetupSuccessfulUpdateScenario();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        cropEntity.OptimalSoil.Should().Be(soilEntity);
    }

    private (UpdateCropCommand command, CropVariety cropEntity, Soil soilEntity, CropVarietyDto expectedDto)
        SetupSuccessfulUpdateScenario()
    {
        var (command, cropEntity) = SetupCropEntityMapping();
        var soilEntity = CreateSoilEntity();
        var expectedDto = CreateCropVarietyDto();

        SetupSoilRepository(soilEntity);
        SetupRepositoryOperations(1);
        SetupEntityToDtoMapping(cropEntity, expectedDto);

        return (command, cropEntity, soilEntity, expectedDto);
    }

    private (UpdateCropCommand command, CropVariety cropEntity) SetupFailingSaveScenario()
    {
        var (command, cropEntity) = SetupCropEntityMapping();
        var soilEntity = CreateSoilEntity();

        SetupSoilRepository(soilEntity);
        SetupRepositoryOperations(0);

        return (command, cropEntity);
    }

    private (UpdateCropCommand command, CropVariety cropEntity) SetupCropEntityMapping()
    {
        var command = CreateUpdateCommand();
        var cropEntity = CreateCropVarietyEntity();

        _mapperMock.Setup(m => m.Map<CropVariety>(command.CropVariety))
            .Returns(cropEntity);

        return (command, cropEntity);
    }

    private void SetupMappingToReturnNull()
    {
        _mapperMock.Setup(m => m.Map<CropVariety>(It.IsAny<CropVarietyUpdateDto>()))
            .Returns((CropVariety)null!);
    }

    private void SetupSoilRepository(Soil soilEntity)
    {
        _repositoryWrapperMock.Setup(r => r.SoilRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Soil, bool>>>(), null))
            .ReturnsAsync(soilEntity);
    }

    private void SetupSoilRepositoryToReturnNull()
    {
        _repositoryWrapperMock.Setup(r => r.SoilRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Soil, bool>>>(), null))
            .ReturnsAsync((Soil)null!);
    }

    private void SetupRepositoryOperations(int saveChangesResult)
    {
        _repositoryWrapperMock.Setup(r => r.CropVarietyRepository.Update(It.IsAny<CropVariety>()));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveChangesResult);
    }

    private void SetupEntityToDtoMapping(CropVariety cropEntity, CropVarietyDto expectedDto)
    {
        _mapperMock.Setup(m => m.Map<CropVarietyDto>(cropEntity))
            .Returns(expectedDto);
    }

    private static UpdateCropCommand CreateUpdateCommand()
    {
        var updateDto = new CropVarietyUpdateDto
        {
            Id = 1,
            Name = "Updated Wheat Variety A",
            CropType = CropType.Wheat,
            FertilizerRequirement = 130,
            WaterRequirement = 520,
            GrowingDuration = 125,
            HarvestYield = 4200,
            MaxTemperature = 26,
            MinTemperature = 9,
            SowingStart = new DayMonth { Day = 20, Month = 4 },
            SowingEnd = new DayMonth { Day = 20, Month = 5 },
            AdditionalNotes = "Updated high yield wheat variety",
            OptimalSoilId = 1
        };

        return new UpdateCropCommand(updateDto);
    }

    private static CropVariety CreateCropVarietyEntity()
    {
        return new CropVariety
        {
            Id = 1,
            Name = "Updated Wheat Variety A",
            CropType = CropType.Wheat,
            FertilizerRequirement = 130,
            WaterRequirement = 520,
            GrowingDuration = 125,
            HarvestYield = 4200,
            MaxTemperature = 26,
            MinTemperature = 9,
            SowingStart = new DayMonth { Day = 20, Month = 4 },
            SowingEnd = new DayMonth { Day = 20, Month = 5 },
            AdditionalNotes = "Updated high yield wheat variety",
            OptimalSoilId = 1
        };
    }

    private static Soil CreateSoilEntity()
    {
        return new Soil
        {
            Id = 1,
            Type = SoilType.Loamy
        };
    }

    private static CropVarietyDto CreateCropVarietyDto()
    {
        return new CropVarietyDto
        {
            Id = 1,
            Name = "Updated Wheat Variety A",
            CropType = CropType.Wheat,
            FertilizerRequirement = 130,
            WaterRequirement = 520,
            GrowingDuration = 125,
            HarvestYield = 4200,
            MaxTemperature = 26,
            MinTemperature = 9,
            SowingStart = new DayMonth { Day = 20, Month = 4 },
            SowingEnd = new DayMonth { Day = 20, Month = 5 },
            AdditionalNotes = "Updated high yield wheat variety",
            OptimalSoil = SoilType.Loamy
        };
    }

    private void VerifyRepositoryInteractions(CropVariety expectedEntity, int expectedSaveResult)
    {
        _repositoryWrapperMock.Verify(r => r.CropVarietyRepository.Update(expectedEntity), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        if (expectedSaveResult > 0) _mapperMock.Verify(m => m.Map<CropVarietyDto>(expectedEntity), Times.Once);
    }

    private void VerifyNoRepositoryInteractions()
    {
        _repositoryWrapperMock.Verify(r => r.SoilRepository
            .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Soil, bool>>>(), null), Times.Never);
        _repositoryWrapperMock.Verify(r => r.CropVarietyRepository.Update(It.IsAny<CropVariety>()), Times.Never);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    private void VerifyNoUpdateOrSaveOperations()
    {
        _repositoryWrapperMock.Verify(r => r.CropVarietyRepository.Update(It.IsAny<CropVariety>()), Times.Never);
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
