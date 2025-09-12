using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.BLL.MediatR.Fields.Soil.GetBySoilType;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;
using SoilEntity = SmartAgroPlan.DAL.Entities.Fields.Soil;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Fields.Soil;

public class GetSoilByTypeHandlerTests
{
    private readonly GetSoilByTypeHandler _handler;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public GetSoilByTypeHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetSoilByTypeHandler(_mapperMock.Object, _repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenSoilExists()
    {
        // Arrange
        var soilType = SoilType.Loamy;
        var soilEntity = new SoilEntity { Id = 1, Type = soilType };
        var soilDto = new SoilDto { Id = 1, Type = soilType };

        _repositoryWrapperMock.Setup(r => r.SoilRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SoilEntity, bool>>>(),
                It.IsAny<Func<IQueryable<SoilEntity>,
                    IIncludableQueryable<SoilEntity, object>>>()))
            .ReturnsAsync(soilEntity);

        _mapperMock.Setup(m => m.Map<SoilDto>(soilEntity))
            .Returns(soilDto);

        var query = new GetSoilByTypeQuery(soilType);

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
        var soilType = SoilType.Loamy;

        _repositoryWrapperMock.Setup(r => r.SoilRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SoilEntity, bool>>>(),
                It.IsAny<Func<IQueryable<SoilEntity>,
                    IIncludableQueryable<SoilEntity, object>>>()))
            .ReturnsAsync((SoilEntity?)null);

        var query = new GetSoilByTypeQuery(soilType);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be($"Не вдалося отримати тип ґрунту з назвою = {soilType}");
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectPredicate()
    {
        // Arrange
        var soilType = SoilType.Sandy;
        var query = new GetSoilByTypeQuery(soilType);
        var soilEntity = new SoilEntity { Id = 1, Type = soilType };
        var soilDto = new SoilDto { Id = 1, Type = soilType };

        Expression<Func<SoilEntity, bool>>? capturedPredicate = null;

        _repositoryWrapperMock.Setup(r => r.SoilRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SoilEntity, bool>>>(),
                It.IsAny<Func<IQueryable<SoilEntity>,
                    IIncludableQueryable<SoilEntity, object>>>()))
            .Callback<Expression<Func<SoilEntity, bool>>, Func<IQueryable<SoilEntity>,
                IIncludableQueryable<SoilEntity, object>>>((predicate, _) =>
                capturedPredicate = predicate)
            .ReturnsAsync(soilEntity);

        _mapperMock.Setup(m => m.Map<SoilDto>(soilEntity))
            .Returns(soilDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(soilDto);
        capturedPredicate.Should().NotBeNull();

        var func = capturedPredicate!.Compile();
        func(new SoilEntity { Type = soilType }).Should().BeTrue();
        func(new SoilEntity { Type = SoilType.Clay }).Should().BeFalse();
    }
}
