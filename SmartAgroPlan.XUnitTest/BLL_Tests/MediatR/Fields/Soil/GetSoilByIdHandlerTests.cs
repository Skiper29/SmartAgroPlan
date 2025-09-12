using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.BLL.MediatR.Fields.Soil.GetById;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Fields.Soil;

public class GetSoilByIdHandlerTests
{
    private readonly GetSoilByIdHandler _handler;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public GetSoilByIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetSoilByIdHandler(_mapperMock.Object, _repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenSoilExists()
    {
        // Arrange
        var soilId = 1;
        var soilEntity = new DAL.Entities.Fields.Soil { Id = soilId, Type = SoilType.Loamy };
        var soilDto = new SoilDto { Id = soilId, Type = SoilType.Loamy };

        _repositoryWrapperMock.Setup(r => r.SoilRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Fields.Soil, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.Fields.Soil>,
                    IIncludableQueryable<DAL.Entities.Fields.Soil, object>>>()))
            .ReturnsAsync(soilEntity);

        _mapperMock.Setup(m => m.Map<SoilDto>(soilEntity))
            .Returns(soilDto);

        var query = new GetSoilByIdQuery(soilId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(soilDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenSoilDoesNotExist()
    {
        // Arrange
        var soilId = 1;

        _repositoryWrapperMock.Setup(r => r.SoilRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Fields.Soil, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.Fields.Soil>,
                    IIncludableQueryable<DAL.Entities.Fields.Soil, object>>>()))
            .ReturnsAsync((DAL.Entities.Fields.Soil?)null);

        var query = new GetSoilByIdQuery(soilId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be($"Не вдалося отримати тип ґрунту з Id = {soilId}");
    }
}
