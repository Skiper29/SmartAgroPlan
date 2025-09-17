using System.Linq.Expressions;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAgroPlan.BLL.MediatR.Fields.Field.Delete;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;
using FieldEntity = SmartAgroPlan.DAL.Entities.Fields.Field;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.MediatR.Fields.Field;

public class DeleteFieldHandlerTests
{
    private readonly DeleteFieldHandler _handler;
    private readonly Mock<ILogger<DeleteFieldHandler>> _loggerMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public DeleteFieldHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILogger<DeleteFieldHandler>>();
        _handler = new DeleteFieldHandler(_repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenFieldDeletedSuccessfully()
    {
        var command = new DeleteFieldCommand(1);
        var fieldEntity = new FieldEntity { Id = command.Id };
        SetupFieldFound(fieldEntity);
        SetupSaveChangesResult(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _repositoryWrapperMock.Verify(r => r.FieldRepository.Delete(fieldEntity), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenFieldNotFound()
    {
        var command = new DeleteFieldCommand(99);
        SetupFieldNotFound();
        var errorMsg = $"Не вдалося знайти поле з Id = {command.Id}";

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        _repositoryWrapperMock.Verify(r => r.FieldRepository.Delete(It.IsAny<FieldEntity>()), Times.Never);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        VerifyLoggerErrorCalled(errorMsg);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var command = new DeleteFieldCommand(2);
        var fieldEntity = new FieldEntity { Id = command.Id };
        SetupFieldFound(fieldEntity);
        SetupSaveChangesResult(0);
        var errorMsg = "Не вдалося видалити поле";

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == errorMsg);
        _repositoryWrapperMock.Verify(r => r.FieldRepository.Delete(fieldEntity), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        VerifyLoggerErrorCalled(errorMsg);
    }

    private void SetupFieldFound(FieldEntity fieldEntity)
    {
        _repositoryWrapperMock.Setup(r =>
                r.FieldRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<FieldEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<FieldEntity>, IIncludableQueryable<FieldEntity, object>>>()))
            .ReturnsAsync(fieldEntity);
    }

    private void SetupFieldNotFound()
    {
        _repositoryWrapperMock.Setup(r =>
                r.FieldRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<FieldEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<FieldEntity>, IIncludableQueryable<FieldEntity, object>>>()))
            .ReturnsAsync((FieldEntity)null!);
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
