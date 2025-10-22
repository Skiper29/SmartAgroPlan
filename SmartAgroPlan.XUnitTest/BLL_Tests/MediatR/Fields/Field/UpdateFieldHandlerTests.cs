using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.BLL.MediatR.Fields.Field.Update;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;
using FieldEntity = SmartAgroPlan.DAL.Entities.Fields.Field;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Fields.Field;

public class UpdateFieldHandlerTests
{
    private readonly UpdateFieldHandler _handler;
    private readonly Mock<ILogger<UpdateFieldHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public UpdateFieldHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILogger<UpdateFieldHandler>>();
        _handler = new UpdateFieldHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenFieldUpdatedSuccessfully()
    {
        // Arrange
        var (command, fieldEntity, fieldDto) = SetupSuccessfulUpdateScenario();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(fieldDto);
        VerifyRepositoryInteractions(fieldEntity, 1);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenFieldNotFound()
    {
        // Arrange
        var command = CreateUpdateCommand();
        const string errorMsg = "Поле не знайдено";

        _repositoryWrapperMock
            .Setup(r => r.FieldRepository.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<FieldEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FieldEntity>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<FieldEntity, object>>>()))
            .ReturnsAsync((FieldEntity?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        VerifyNoRepositoryInteractions();
        VerifyLoggerErrorCalled(errorMsg);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappingReturnsNull()
    {
        // Arrange
        var command = CreateUpdateCommand();
        const string errorMsg = "Не вдалося відобразити DTO в сутність поля";

        var existingField = new FieldEntity { Id = 1, CreatedAt = DateTime.UtcNow };
        _repositoryWrapperMock
            .Setup(r => r.FieldRepository.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<FieldEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FieldEntity>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<FieldEntity, object>>>()))
            .ReturnsAsync(existingField);

        _mapperMock.Setup(m => m.Map<FieldEntity>(command.UpdatedField)).Returns((FieldEntity)null!);

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
        var (command, fieldEntity, _) = SetupSuccessfulUpdateScenario();
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);
        const string errorMsg = "Не вдалося оновити поле";

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        VerifyRepositoryInteractions(fieldEntity, 0);
        VerifyLoggerErrorCalled(errorMsg);
    }

    [Fact]
    public async Task Handle_ShouldUpdateFieldCorrectly_WhenAllPropertiesChanged()
    {
        // Arrange
        var command = CreateUpdateCommandWithAllProperties();
        var fieldEntity = CreateFieldEntityWithAllProperties();
        var fieldDto = CreateFieldDtoWithAllProperties();

        SetupSuccessfulUpdateScenarioMocks(command, fieldEntity, fieldDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(fieldDto);
        _repositoryWrapperMock.Verify(r => r.FieldRepository.Update(fieldEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryUpdateOnce_WhenValidRequest()
    {
        // Arrange
        var (command, fieldEntity, _) = SetupSuccessfulUpdateScenario();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryWrapperMock.Verify(r => r.FieldRepository.Update(fieldEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotCallMapperForResult_WhenSaveChangesFails()
    {
        // Arrange
        var (command, _, _) = SetupSuccessfulUpdateScenario();
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<FieldDto>(It.IsAny<FieldEntity>()), Times.Never);
    }

    private (UpdateFieldCommand command, FieldEntity fieldEntity, FieldDto fieldDto) SetupSuccessfulUpdateScenario()
    {
        var command = CreateUpdateCommand();
        var fieldEntity = CreateFieldEntity();
        var fieldDto = CreateFieldDto();

        SetupSuccessfulUpdateScenarioMocks(command, fieldEntity, fieldDto);

        return (command, fieldEntity, fieldDto);
    }

    private void SetupSuccessfulUpdateScenarioMocks(UpdateFieldCommand command, FieldEntity fieldEntity,
        FieldDto fieldDto)
    {
        // Mock GetFirstOrDefaultAsync to return existing field
        var existingField = new FieldEntity
        {
            Id = command.UpdatedField.Id,
            Name = "Old Name",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5),
            CurrentCropId = fieldEntity.CurrentCropId,
            SoilId = fieldEntity.SoilId,
            Location = "Old Location",
            FieldType = fieldEntity.FieldType
        };

        _repositoryWrapperMock
            .Setup(r => r.FieldRepository.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<FieldEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FieldEntity>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<FieldEntity, object>>>()))
            .ReturnsAsync(existingField);

        _mapperMock.Setup(m => m.Map<FieldEntity>(command.UpdatedField)).Returns(fieldEntity);
        _repositoryWrapperMock.Setup(r => r.FieldRepository.Update(fieldEntity));
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<FieldDto>(fieldEntity)).Returns(fieldDto);
    }

    private static UpdateFieldCommand CreateUpdateCommand()
    {
        var updateDto = new FieldUpdateDto
        {
            Id = 1,
            Name = "Updated Field Name",
            SoilId = 2,
            Location = "Updated Location",
            BoundaryGeoJson = null,
            FieldType = FieldType.Pasture,
            CurrentCropId = 2
        };
        return new UpdateFieldCommand(updateDto);
    }

    private static UpdateFieldCommand CreateUpdateCommandWithAllProperties()
    {
        var updateDto = new FieldUpdateDto
        {
            Id = 1,
            Name = "Complete Updated Field",
            SoilId = 3,
            Location = "New Complete Location",
            FieldType = FieldType.Orchard,
            CurrentCropId = 3
        };
        return new UpdateFieldCommand(updateDto);
    }

    private static FieldEntity CreateFieldEntity()
    {
        return new FieldEntity
        {
            Id = 1,
            Name = "Updated Field Name",
            SoilId = 2,
            Soil = null,
            CurrentCrop = null,
            CropHistories = null,
            Location = "Updated Location",
            SowingDate = null,
            Boundary = null,
            FieldType = FieldType.Pasture,
            CurrentCropId = 2,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static FieldEntity CreateFieldEntityWithAllProperties()
    {
        return new FieldEntity
        {
            Id = 1,
            Name = "Complete Updated Field",
            SoilId = 3,
            Soil = null,
            CurrentCrop = null,
            CropHistories = null,
            Location = "New Complete Location",
            SowingDate = null,
            Boundary = null,
            FieldType = FieldType.Orchard,
            CurrentCropId = 3,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static FieldDto CreateFieldDto()
    {
        return new FieldDto
        {
            Id = 1,
            Name = "Updated Field Name",
            Soil = null,
            CurrentCrop = null,
            Location = "Updated Location",
            BoundaryGeoJson = null,
            FieldType = FieldType.Pasture,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static FieldDto CreateFieldDtoWithAllProperties()
    {
        return new FieldDto
        {
            Id = 1,
            Name = "Complete Updated Field",
            Soil = null,
            CurrentCrop = null,
            Location = "New Complete Location",
            BoundaryGeoJson = "{\"type\":\"Polygon\",\"coordinates\":[[[0,0],[1,0],[1,1],[0,1],[0,0]]]}",
            FieldType = FieldType.Orchard,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow
        };
    }

    private void VerifyRepositoryInteractions(FieldEntity expectedEntity, int expectedSaveResult)
    {
        _repositoryWrapperMock.Verify(r => r.FieldRepository.Update(expectedEntity), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        if (expectedSaveResult > 0) _mapperMock.Verify(m => m.Map<FieldDto>(expectedEntity), Times.Once);
    }

    private void VerifyNoRepositoryInteractions()
    {
        _repositoryWrapperMock.Verify(r => r.FieldRepository.Update(It.IsAny<FieldEntity>()), Times.Never);
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
