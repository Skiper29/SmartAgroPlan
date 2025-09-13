using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.GetAll;

public class GetAllFieldsHandler : IRequestHandler<GetAllFieldsQuery, Result<IEnumerable<FieldDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetAllFieldsHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<IEnumerable<FieldDto>>> Handle(GetAllFieldsQuery request,
        CancellationToken cancellationToken)
    {
        var fields = await _repository.FieldRepository.GetAllAsync(
            include: e => e
                .Include(f => f.Soil)
                .Include(f => f.CurrentCrop)!);
        var fieldsDto = _mapper.Map<IEnumerable<FieldDto>>(fields);
        return Result.Ok(fieldsDto);
    }
}
