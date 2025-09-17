using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.GetById;

public class GetFieldByIdHandler : IRequestHandler<GetFieldByIdQuery, Result<FieldDto>>
{
    private readonly ILogger<GetFieldByIdHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetFieldByIdHandler(
        ILogger<GetFieldByIdHandler> logger,
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper)
    {
        _logger = logger;
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<FieldDto>> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        var field = await _repositoryWrapper.FieldRepository.GetFirstOrDefaultAsync(
            f => f.Id == request.Id,
            e => e
                .Include(f => f.Soil)
                .Include(f => f.CurrentCrop)!);

        if (field == null)
        {
            var errorMsg = $"Не вдалося отримати поле з Id = {request.Id}";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<FieldDto>(field));
    }
}
