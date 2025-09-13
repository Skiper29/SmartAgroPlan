using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NetTopologySuite.Geometries;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.BLL.MediatR.Fields.Field.GetAll;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;
using FieldEntity = SmartAgroPlan.DAL.Entities.Fields.Field;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Fields.Field;

public class GetAllFieldsHandlerTests
{
    private readonly GetAllFieldsHandler _handler;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public GetAllFieldsHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _handler = new GetAllFieldsHandler(_mapperMock.Object, _repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkWithAllFields_WhenFieldsExist()
    {
        // Arrange
        var (query, _, fieldDtos) = SetupSuccessfulGetAllScenario();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(fieldDtos);
        result.Value.Should().HaveCount(2);
        VerifyRepositoryInteractions();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkWithEmptyCollection_WhenNoFieldsExist()
    {
        // Arrange
        var (query, fieldDtos) = SetupEmptyFieldsScenario();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(fieldDtos);
        result.Value.Should().BeEmpty();
        VerifyRepositoryInteractions();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkWithSingleField_WhenOnlyOneFieldExists()
    {
        // Arrange
        var (query, fieldDtos) = SetupSingleFieldScenario();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(fieldDtos);
        result.Value.Should().HaveCount(1);
        VerifyRepositoryInteractions();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkWithFieldsWithoutCurrentCrop_WhenFieldsHaveNullCurrentCrop()
    {
        // Arrange
        var (query, fieldDtos) = SetupFieldsWithoutCurrentCropScenario();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(fieldDtos);
        result.Value.Should().HaveCount(1);
        result.Value.First().CurrentCrop.Should().BeNull();
        VerifyRepositoryInteractions();
    }

    [Fact]
    public async Task Handle_ShouldIncludeRelatedEntities_WhenCallingRepository()
    {
        // Arrange
        var (query, fieldEntities, _) = SetupSuccessfulGetAllScenario();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryWrapperMock.Verify(
            r => r.FieldRepository.GetAllAsync(
                It.IsAny<Expression<Func<FieldEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FieldEntity>, IIncludableQueryable<FieldEntity, object?>>>()!),
            Times.Once);
        VerifyMappingInteractions(fieldEntities);
    }

    private (GetAllFieldsQuery query, List<FieldEntity> fieldEntities, List<FieldDto> fieldDtos)
        SetupSuccessfulGetAllScenario()
    {
        var query = CreateGetAllFieldsQuery();
        var fieldEntities = CreateMultipleFieldEntities();
        var fieldDtos = CreateMultipleFieldDtos();

        SetupRepositoryGetAllAsync(fieldEntities);
        SetupMapperMapping(fieldEntities, fieldDtos);

        return (query, fieldEntities, fieldDtos);
    }

    private (GetAllFieldsQuery query, List<FieldDto> fieldDtos) SetupEmptyFieldsScenario()
    {
        var query = CreateGetAllFieldsQuery();
        var fieldEntities = new List<FieldEntity>();
        var fieldDtos = new List<FieldDto>();

        SetupRepositoryGetAllAsync(fieldEntities);
        SetupMapperMapping(fieldEntities, fieldDtos);

        return (query, fieldDtos);
    }

    private (GetAllFieldsQuery query, List<FieldDto> fieldDtos)
        SetupSingleFieldScenario()
    {
        var query = CreateGetAllFieldsQuery();
        var fieldEntities = new List<FieldEntity> { CreateMultipleFieldEntities()[0] };
        var fieldDtos = new List<FieldDto> { CreateMultipleFieldDtos()[0] };

        SetupRepositoryGetAllAsync(fieldEntities);
        SetupMapperMapping(fieldEntities, fieldDtos);

        return (query, fieldDtos);
    }

    private (GetAllFieldsQuery query, List<FieldDto> fieldDtos)
        SetupFieldsWithoutCurrentCropScenario()
    {
        var query = CreateGetAllFieldsQuery();
        var fieldEntity = CreateMultipleFieldEntities()[0];
        var fieldDto = CreateMultipleFieldDtos()[0];
        fieldEntity.CurrentCrop = null;
        fieldDto.CurrentCrop = null;
        var fieldEntities = new List<FieldEntity> { fieldEntity };
        var fieldDtos = new List<FieldDto> { fieldDto };

        SetupRepositoryGetAllAsync(fieldEntities);
        SetupMapperMapping(fieldEntities, fieldDtos);

        return (query, fieldDtos);
    }

    private void SetupRepositoryGetAllAsync(List<FieldEntity> fieldEntities)
    {
        _repositoryWrapperMock.Setup(r => r.FieldRepository.GetAllAsync(
                It.IsAny<Expression<Func<FieldEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FieldEntity>, IIncludableQueryable<FieldEntity, object?>>>()!))
            .ReturnsAsync(fieldEntities);
    }

    private void SetupMapperMapping(List<FieldEntity> fieldEntities, List<FieldDto> fieldDtos)
    {
        _mapperMock.Setup(m => m.Map<IEnumerable<FieldDto>>(fieldEntities)).Returns(fieldDtos);
    }

    private static GetAllFieldsQuery CreateGetAllFieldsQuery()
    {
        return new GetAllFieldsQuery();
    }

    private static List<FieldEntity> CreateMultipleFieldEntities()
    {
        return
        [
            new FieldEntity
            {
                Id = 1,
                Name = "Main Field",
                Location = "Central Area",
                SowingDate = DateTime.Now.AddDays(-60),
                Boundary = CreateTestPolygon(),
                FieldType = FieldType.Arable,
                CurrentCropId = 1,
                SoilId = 1,
                Soil = CreateSoilEntity(1, SoilType.Loamy),
                CurrentCrop = CreateCropVarietyEntity(1, "Wheat Premium")
            },

            new FieldEntity
            {
                Id = 2,
                Name = "North Field",
                Location = "North Section",
                SowingDate = DateTime.Now.AddDays(-30),
                Boundary = CreateTestPolygon(),
                FieldType = FieldType.Arable,
                CurrentCropId = 2,
                SoilId = 2,
                Soil = CreateSoilEntity(2, SoilType.Sandy),
                CurrentCrop = CreateCropVarietyEntity(2, "Corn Premium")
            }
        ];
    }

    private static List<FieldDto> CreateMultipleFieldDtos()
    {
        return
        [
            new FieldDto
            {
                Id = 1,
                Name = "Main Field",
                Location = "Central Area",
                BoundaryGeoJson = "{\"type\":\"Polygon\",\"coordinates\":[[[0,0],[1,0],[1,1],[0,1],[0,0]]]}",
                FieldType = FieldType.Arable,
                CurrentCrop = CreateCropVarietyDto(1, "Wheat Premium"),
                Soil = CreateSoilDto(1, SoilType.Loamy)
            },

            new FieldDto
            {
                Id = 2,
                Name = "North Field",
                Location = "North Section",
                BoundaryGeoJson = "{\"type\":\"Polygon\",\"coordinates\":[[[0,0],[1,0],[1,1],[0,1],[0,0]]]}",
                FieldType = FieldType.Arable,
                CurrentCrop = CreateCropVarietyDto(2, "Corn Premium"),
                Soil = CreateSoilDto(2, SoilType.Sandy)
            }
        ];
    }

    private static DAL.Entities.Fields.Soil CreateSoilEntity(int id, SoilType soilType)
    {
        return new DAL.Entities.Fields.Soil
        {
            Id = id,
            Type = soilType
        };
    }

    private static SoilDto CreateSoilDto(int id, SoilType soilType)
    {
        return new SoilDto
        {
            Id = id,
            Type = soilType
        };
    }

    private static CropVariety CreateCropVarietyEntity(int id, string name)
    {
        return new CropVariety
        {
            Id = id,
            Name = name,
            CropType = CropType.Wheat
        };
    }

    private static CropVarietyDto CreateCropVarietyDto(int id, string name)
    {
        return new CropVarietyDto
        {
            Id = id,
            Name = name,
            CropType = CropType.Wheat
        };
    }

    private static Polygon CreateTestPolygon()
    {
        var coordinates = new Coordinate[]
        {
            new(0, 0),
            new(1, 0),
            new(1, 1),
            new(0, 1),
            new(0, 0)
        };
        var factory = new GeometryFactory();
        return factory.CreatePolygon(coordinates);
    }

    private void VerifyRepositoryInteractions()
    {
        _repositoryWrapperMock.Verify(
            r => r.FieldRepository.GetAllAsync(
                It.IsAny<Expression<Func<FieldEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FieldEntity>, IIncludableQueryable<FieldEntity, object?>>>()!),
            Times.Once);
    }

    private void VerifyMappingInteractions(List<FieldEntity> fieldEntities)
    {
        _mapperMock.Verify(m => m.Map<IEnumerable<FieldDto>>(fieldEntities), Times.Once);
    }
}
