using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.BLL.MediatR.Fields.Field.GetById;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;
using FieldEntity = SmartAgroPlan.DAL.Entities.Fields.Field;
using SoilEntity = SmartAgroPlan.DAL.Entities.Fields.Soil;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Fields.Field;

public class GetFieldByIdHandlerTests
{
    private readonly GetFieldByIdHandler _handler;
    private readonly Mock<ILogger<GetFieldByIdHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public GetFieldByIdHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILogger<GetFieldByIdHandler>>();
        _handler = new GetFieldByIdHandler(_loggerMock.Object, _mapperMock.Object, _repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenFieldExists()
    {
        // Arrange
        var fieldEntity = CreateFieldEntity();
        var expectedDto = CreateFieldDto();
        var query = new GetFieldByIdQuery(fieldEntity.Id);

        _repositoryWrapperMock.Setup(r => r.FieldRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FieldEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FieldEntity>, IIncludableQueryable<FieldEntity, object>>>()))
            .ReturnsAsync(fieldEntity);

        _mapperMock.Setup(m => m.Map<FieldDto>(fieldEntity))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenFieldDoesNotExist()
    {
        // Arrange
        var errorMsg = "Не вдалося отримати поле з Id = 1";

        _repositoryWrapperMock.Setup(r => r.FieldRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FieldEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FieldEntity>, IIncludableQueryable<FieldEntity, object>>>()))
            .ReturnsAsync((FieldEntity?)null);

        var query = new GetFieldByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        VerifyLoggerErrorCalled(errorMsg);
    }

    [Fact]
    public async Task Handle_ShouldIncludeSoilAndCurrentCrop_WhenRetrievingField()
    {
        // Arrange
        var fieldEntity = CreateFieldEntity();
        var expectedDto = CreateFieldDto();
        var query = new GetFieldByIdQuery(fieldEntity.Id);

        _repositoryWrapperMock.Setup(r => r.FieldRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FieldEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FieldEntity>, IIncludableQueryable<FieldEntity, object>>>()))
            .ReturnsAsync(fieldEntity);

        _mapperMock.Setup(m => m.Map<FieldDto>(fieldEntity))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDto);
        _repositoryWrapperMock.Verify(r => r.FieldRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<FieldEntity, bool>>>(),
            It.Is<Func<IQueryable<FieldEntity>, IIncludableQueryable<FieldEntity, object>>>(include =>
                include != null)), Times.Once);
    }

    private static FieldEntity CreateFieldEntity()
    {
        return
            new FieldEntity
            {
                Id = 1,
                FieldType = FieldType.Arable,
                CurrentCropId = 1,
                SoilId = 1,
                Soil = new SoilEntity
                {
                    Id = 1,
                    Type = SoilType.Loamy
                },
                CurrentCrop = new CropVariety
                {
                    Id = 1,
                    CropType = CropType.Wheat
                }
            };
    }

    private static FieldDto CreateFieldDto()
    {
        return new FieldDto
        {
            Id = 1,
            FieldType = FieldType.Arable,
            Soil = new SoilDto
            {
                Id = 1,
                Type = SoilType.Loamy
            },
            CurrentCrop = new CropVarietyDto
            {
                Id = 1,
                CropType = CropType.Wheat
            }
        };
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
