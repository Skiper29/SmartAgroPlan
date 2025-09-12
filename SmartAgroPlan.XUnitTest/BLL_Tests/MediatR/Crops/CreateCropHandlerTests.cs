using AutoMapper;
using FluentAssertions;
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
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public CreateCropHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _handler = new CreateCropHandler(_mapperMock.Object, _repositoryWrapperMock.Object);
    }


    [Fact]
    public async Task Handle_ShouldReturnOk_WhenCropCreatedSuccessfully()
    {
        // Arrange
        var (cropCreateDto, cropEntity, cropDto) = PrepareCropData();
        _mapperMock.Setup(m => m.Map<CropVariety>(cropCreateDto)).Returns(cropEntity);
        _repositoryWrapperMock.Setup(r => r.CropVarietyRepository.CreateAsync(cropEntity)).ReturnsAsync(cropEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<CropVarietyDto>(cropEntity)).Returns(cropDto);
        var command = new CreateCropCommand(cropCreateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cropDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappingReturnsNull()
    {
        // Arrange
        var (cropCreateDto, _, _) = PrepareCropData();

        _mapperMock.Setup(m => m.Map<CropVariety>(cropCreateDto)).Returns((CropVariety)null!);
        var command = new CreateCropCommand(cropCreateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Не вдалося відобразити DTO в сутність сорту культури");
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        // Arrange
        var (cropCreateDto, cropEntity, _) = PrepareCropData();
        _mapperMock.Setup(m => m.Map<CropVariety>(cropCreateDto)).Returns(cropEntity);
        _repositoryWrapperMock.Setup(r => r.CropVarietyRepository.CreateAsync(cropEntity)).ReturnsAsync(cropEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);
        var command = new CreateCropCommand(cropCreateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Не вдалося створити сорт культури");
    }

    private static (CropVarietyCreateDto cropCreateDto, CropVariety cropEntity, CropVarietyDto cropDto)
        PrepareCropData()
    {
        var cropCreateDto = new CropVarietyCreateDto
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

        var cropEntity = new CropVariety
        {
            Id = 1,
            Name = cropCreateDto.Name,
            CropType = cropCreateDto.CropType,
            FertilizerRequirement = cropCreateDto.FertilizerRequirement,
            WaterRequirement = cropCreateDto.WaterRequirement,
            GrowingDuration = cropCreateDto.GrowingDuration,
            HarvestYield = cropCreateDto.HarvestYield,
            MaxTemperature = cropCreateDto.MaxTemperature,
            MinTemperature = cropCreateDto.MinTemperature,
            SowingStart = cropCreateDto.SowingStart,
            SowingEnd = cropCreateDto.SowingEnd,
            AdditionalNotes = cropCreateDto.AdditionalNotes,
            OptimalSoilId = cropCreateDto.OptimalSoilId
        };

        var cropDto = new CropVarietyDto
        {
            Id = cropEntity.Id,
            Name = cropEntity.Name,
            CropType = cropEntity.CropType,
            FertilizerRequirement = cropEntity.FertilizerRequirement,
            WaterRequirement = cropEntity.WaterRequirement,
            GrowingDuration = cropEntity.GrowingDuration,
            HarvestYield = cropEntity.HarvestYield,
            MaxTemperature = cropEntity.MaxTemperature,
            MinTemperature = cropEntity.MinTemperature,
            SowingStart = cropEntity.SowingStart,
            SowingEnd = cropEntity.SowingEnd,
            AdditionalNotes = cropEntity.AdditionalNotes,
            OptimalSoil = SoilType.Loamy
        };


        return (cropCreateDto, cropEntity, cropDto);
    }
}
