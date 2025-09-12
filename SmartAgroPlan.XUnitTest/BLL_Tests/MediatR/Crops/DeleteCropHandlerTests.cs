using System.Linq.Expressions;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAgroPlan.BLL.MediatR.Crops.Delete;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Crops;

public class DeleteCropHandlerTests
{
    private readonly DeleteCropHandler _handler;
    private readonly Mock<ILogger<DeleteCropHandler>> _loggerMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public DeleteCropHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILogger<DeleteCropHandler>>();
        _handler = new DeleteCropHandler(null!, _repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenCropDeletedSuccessfully()
    {
        // Arrange
        var command = new DeleteCropCommand(1);
        var cropEntity = new CropVariety { Id = command.Id };
        SetupCropFound(cropEntity);
        SetupSaveChangesResult(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _repositoryWrapperMock.Verify(r => r.CropVarietyRepository.Delete(cropEntity), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCropNotFound()
    {
        // Arrange
        var command = new DeleteCropCommand(99);
        SetupCropNotFound();
        var errorMsg = $"Не вдалося отримати сорт культури з Id = {command.Id}";

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        _repositoryWrapperMock.Verify(r => r.CropVarietyRepository.Delete(It.IsAny<CropVariety>()), Times.Never);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        VerifyLoggerErrorCalled(errorMsg);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        // Arrange
        var command = new DeleteCropCommand(2);
        var cropEntity = new CropVariety { Id = command.Id };
        SetupCropFound(cropEntity);
        SetupSaveChangesResult(0);
        var errorMsg = "Не вдалося видалити сорт культури";

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        _repositoryWrapperMock.Verify(r => r.CropVarietyRepository.Delete(cropEntity), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        VerifyLoggerErrorCalled(errorMsg);
    }

    private void SetupCropFound(CropVariety cropEntity)
    {
        _repositoryWrapperMock.Setup(r =>
                r.CropVarietyRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<CropVariety, bool>>>(),
                    It.IsAny<Func<IQueryable<CropVariety>, IIncludableQueryable<CropVariety, object>>>()))
            .ReturnsAsync(cropEntity);
    }

    private void SetupCropNotFound()
    {
        _repositoryWrapperMock.Setup(r =>
                r.CropVarietyRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<CropVariety, bool>>>(),
                    It.IsAny<Func<IQueryable<CropVariety>, IIncludableQueryable<CropVariety, object>>>()))
            .ReturnsAsync((CropVariety)null!);
    }

    private void SetupSaveChangesResult(int result)
    {
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(result);
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
