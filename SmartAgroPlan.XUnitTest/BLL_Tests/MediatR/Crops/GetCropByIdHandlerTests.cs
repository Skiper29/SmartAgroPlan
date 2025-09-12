using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.MediatR.Crops.GetById;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Crops;

public class GetCropByIdHandlerTests
{
    private readonly GetCropByIdHandler _handler;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public GetCropByIdHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _handler = new GetCropByIdHandler(_mapperMock.Object, _repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenCropExists()
    {
        // Arrange
        var (crops, cropDtos) = PrepareCropData();
        var crop = crops.First();
        var cropDto = cropDtos.First();
        _repositoryWrapperMock.Setup(r => r.CropVarietyRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<CropVariety, bool>>>(),
                It.IsAny<Func<IQueryable<CropVariety>, IIncludableQueryable<CropVariety, object>>>()))
            .ReturnsAsync(crop);
        _mapperMock.Setup(m => m.Map<CropVarietyDto>(crop))
            .Returns(cropDto);
        var query = new GetCropByIdQuery(crop.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cropDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenCropDoesNotExist()
    {
        // Arrange
        _repositoryWrapperMock.Setup(r => r.CropVarietyRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<CropVariety, bool>>>(),
                It.IsAny<Func<IQueryable<CropVariety>, IIncludableQueryable<CropVariety, object>>>()))
            .ReturnsAsync((CropVariety?)null);
        var query = new GetCropByIdQuery(999); // Non-existent ID

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be("Не вдалося отримати сорт культури з Id = 999");
    }

    private (List<CropVariety>, List<CropVarietyDto>) PrepareCropData()
    {
        var crops = new List<CropVariety>
        {
            new()
            {
                Id = 1, Name = "Wheat Triticum", AdditionalNotes = "A cereal grain",
                OptimalSoil = new Soil { Id = 1, Type = SoilType.Clay }
            },
            new()
            {
                Id = 2, Name = "Corn Zea mays", AdditionalNotes = "A staple food",
                OptimalSoil = new Soil { Id = 2, Type = SoilType.Sandy }
            }
        };

        var cropDtos = new List<CropVarietyDto>
        {
            new() { Id = 1, Name = "Wheat Triticum", AdditionalNotes = "A cereal grain", OptimalSoil = SoilType.Clay },
            new() { Id = 2, Name = "Corn Zea mays", AdditionalNotes = "A staple food", OptimalSoil = SoilType.Sandy }
        };

        return (crops, cropDtos);
    }
}
