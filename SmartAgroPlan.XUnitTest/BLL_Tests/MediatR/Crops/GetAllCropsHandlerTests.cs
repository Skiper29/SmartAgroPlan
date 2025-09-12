using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.MediatR.Crops.GetAll;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Crops;

public class GetAllCropsHandlerTests
{
    private readonly GetAllCropsHandler _handler;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public GetAllCropsHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _handler = new GetAllCropsHandler(_mapperMock.Object, _repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenCropsExist()
    {
        // Arrange
        var (crops, cropDtos) = PrepareCropData();
        _repositoryWrapperMock.Setup(r => r.CropVarietyRepository.GetAllAsync(
                It.IsAny<Expression<Func<CropVariety, bool>>>(),
                It.IsAny<Func<IQueryable<CropVariety>, IIncludableQueryable<CropVariety, object>>>()))
            .ReturnsAsync(crops);
        _mapperMock.Setup(m => m.Map<IEnumerable<CropVarietyDto>>(crops))
            .Returns(cropDtos);
        var query = new GetAllCropsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cropDtos);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenNoCropsExist()
    {
        // Arrange
        _repositoryWrapperMock.Setup(r => r.CropVarietyRepository.GetAllAsync(
                It.IsAny<Expression<Func<CropVariety, bool>>>(),
                It.IsAny<Func<IQueryable<CropVariety>, IIncludableQueryable<CropVariety, object>>>()))
            .ReturnsAsync(new List<CropVariety>());
        var query = new GetAllCropsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Не вдалося отримати сорти культур");
    }

    private (List<CropVariety>, List<CropVarietyDto>) PrepareCropData()
    {
        return ([
            new CropVariety
            {
                Id = 1,
                Name = "Wheat",
                OptimalSoilId = 1,
                OptimalSoil = new Soil
                {
                    Id = 1,
                    Type = SoilType.Loamy
                }
            },

            new CropVariety
            {
                Id = 2,
                Name = "Corn",
                OptimalSoilId = 2,
                OptimalSoil = new Soil
                {
                    Id = 2,
                    Type = SoilType.Sandy
                }
            }
        ], [
            new CropVarietyDto
            {
                Id = 1,
                Name = "Wheat",
                OptimalSoil = SoilType.Loamy
            },

            new CropVarietyDto
            {
                Id = 2,
                Name = "Corn",
                OptimalSoil = SoilType.Sandy
            }
        ]);
    }
}
