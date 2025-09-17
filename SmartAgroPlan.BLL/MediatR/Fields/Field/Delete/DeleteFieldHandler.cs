using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.Delete;

public class DeleteFieldHandler : IRequestHandler<DeleteFieldCommand, Result<Unit>>
{
    private readonly ILogger<DeleteFieldHandler> _logger;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteFieldHandler(
        IRepositoryWrapper repositoryWrapper,
        ILogger<DeleteFieldHandler> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteFieldCommand request, CancellationToken cancellationToken)
    {
        var fieldEntity = await _repositoryWrapper.FieldRepository
            .GetFirstOrDefaultAsync(f => f.Id == request.Id);

        if (fieldEntity is null)
        {
            var errorMsg = $"Не вдалося знайти поле з Id = {request.Id}";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _repositoryWrapper.FieldRepository.Delete(fieldEntity);
        var isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSuccess)
        {
            const string errorMsg = "Не вдалося видалити поле";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(Unit.Value);
    }
}
