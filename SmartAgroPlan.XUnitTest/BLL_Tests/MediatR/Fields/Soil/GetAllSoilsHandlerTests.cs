using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.BLL.MediatR.Fields.Soil.GetAll;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

using SoilEntity = SmartAgroPlan.DAL.Entities.Fields.Soil;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Fields.Soil;

public class GetAllSoilsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllSoilsHandler _handler;

    public GetAllSoilsHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllSoilsHandler(_repositoryWrapperMock.Object, _mapperMock.Object);
    }
        
    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenSoilsExist()
    {
        // Arrange
        var soils = PrepareSoilEntities();
        var soilDtos = PrepareSoilDtos();
        _repositoryWrapperMock.Setup(r => r.SoilRepository.GetAllAsync(
                It.IsAny<Expression<Func<SoilEntity, bool>>>(),
                It.IsAny<Func<IQueryable<SoilEntity>, IIncludableQueryable<SoilEntity, object>>>()))
            .ReturnsAsync(soils);
        _mapperMock.Setup(m => m.Map<IEnumerable<SoilDto>>(soils))
            .Returns(soilDtos);
        var query = new GetAllSoilsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(soilDtos);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenNoSoilsExist()
    {
        // Arrange
        _repositoryWrapperMock.Setup(r => r.SoilRepository.GetAllAsync(
                It.IsAny<Expression<Func<SoilEntity, bool>>>(),
                It.IsAny<Func<IQueryable<SoilEntity>, IIncludableQueryable<SoilEntity, object>>>()))
            .ReturnsAsync(new List<SoilEntity>());
        var query = new GetAllSoilsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Не вдалося отримати типи ґрунтів");
    }
        
    private List<SoilEntity> PrepareSoilEntities()
    {
        return [new SoilEntity { Id = 1 }, new SoilEntity { Id = 2 }];
    }

    private List<SoilDto> PrepareSoilDtos()
    {
        return
        [
            new SoilDto { Id = 1 },
            new SoilDto { Id = 2 }
        ];
    }
}