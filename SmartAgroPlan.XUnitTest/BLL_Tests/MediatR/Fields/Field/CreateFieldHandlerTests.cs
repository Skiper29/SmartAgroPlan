using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.BLL.MediatR.Fields.Field.Create;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;
using FieldEntity = SmartAgroPlan.DAL.Entities.Fields.Field;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Fields.Field;

public class CreateFieldHandlerTests
{
    private readonly CreateFieldHandler _handler;
    private readonly Mock<ILogger<CreateFieldHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public CreateFieldHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILogger<CreateFieldHandler>>();
        _handler = new CreateFieldHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenFieldCreatedSuccessfully()
    {
        // Arrange
        var (command, fieldEntity, fieldDto) = SetupSuccessfulCreateScenario();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(fieldDto);
        VerifyRepositoryInteractions(fieldEntity, 1);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappingReturnsNull()
    {
        // Arrange
        var command = CreateCreateCommand();
        const string errorMsg = "Не вдалося відобразити DTO в сутність поля";

        _mapperMock.Setup(m => m.Map<FieldEntity>(command.NewField)).Returns((FieldEntity)null!);

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
        var (command, fieldEntity, _) = SetupSuccessfulCreateScenario();
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);
        const string errorMsg = "Не вдалося створити поле";

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        VerifyRepositoryInteractions(fieldEntity, 0);
        VerifyLoggerErrorCalled(errorMsg);
    }

    private (CreateFieldCommand command, FieldEntity fieldEntity, FieldDto fieldDto) SetupSuccessfulCreateScenario()
    {
        var command = CreateCreateCommand();
        var fieldEntity = CreateFieldEntity();
        var fieldDto = CreateFieldDtoDto();

        _mapperMock.Setup(m => m.Map<FieldEntity>(command.NewField)).Returns(fieldEntity);
        _repositoryWrapperMock.Setup(r => r.FieldRepository.CreateAsync(fieldEntity)).ReturnsAsync(fieldEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<FieldDto>(fieldEntity)).Returns(fieldDto);

        return (command, fieldEntity, fieldDto);
    }

    private static CreateFieldCommand CreateCreateCommand()
    {
        var createDto = new FieldCreateDto
        {
            Name = "Wheat Variety A",
            SoilId = 1,
            Location = "Field Location",
            BoundaryGeoJson = null,
            FieldType = FieldType.Arable,
            CurrentCropId = 1
        };
        return new CreateFieldCommand(createDto);
    }

    private static FieldEntity CreateFieldEntity()
    {
        return new FieldEntity
        {
            Id = 1,
            Name = "Wheat Variety A",
            SoilId = 1,
            Soil = null,
            CurrentCrop = null,
            CropHistories = null,
            Location = "Field Location",
            SowingDate = null,
            Boundary = null,
            FieldType = FieldType.Arable,
            CurrentCropId = 1
        };
    }

    private static FieldDto CreateFieldDtoDto()
    {
        return new FieldDto
        {
            Id = 1,
            Name = "Wheat Variety A",
            Soil = null,
            CurrentCrop = null,
            Location = "Field Location",
            BoundaryGeoJson = null,
            FieldType = FieldType.Arable
        };
    }

    private void VerifyRepositoryInteractions(FieldEntity expectedEntity, int expectedSaveResult)
    {
        _repositoryWrapperMock.Verify(r => r.FieldRepository.CreateAsync(expectedEntity), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        if (expectedSaveResult > 0) _mapperMock.Verify(m => m.Map<FieldDto>(expectedEntity), Times.Once);
    }

    private void VerifyNoRepositoryInteractions()
    {
        _repositoryWrapperMock.Verify(r => r.FieldRepository.CreateAsync(It.IsAny<FieldEntity>()), Times.Never);
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
