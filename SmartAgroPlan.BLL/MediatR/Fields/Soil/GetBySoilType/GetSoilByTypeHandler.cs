using AutoMapper;
using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.Soil.GetBySoilType;

public class GetSoilByTypeHandler : IRequestHandler<GetSoilByTypeQuery, Result<SoilDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    
    public GetSoilByTypeHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }
    
    public async Task<Result<SoilDto>> Handle(GetSoilByTypeQuery request, CancellationToken cancellationToken)
    {
        var soil = await _repositoryWrapper.SoilRepository
            .GetFirstOrDefaultAsync(s => s.Type == request.SoilType);

        if (soil is null)
        {
            var errorMsg = $"Не вдалося отримати тип ґрунту з назвою = {request.SoilType}";
            return Result.Fail(new Error(errorMsg));
        }
        return Result.Ok(_mapper.Map<SoilDto>(soil));
    }
}