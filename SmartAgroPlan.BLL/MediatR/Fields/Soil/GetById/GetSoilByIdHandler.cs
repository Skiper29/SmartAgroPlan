using AutoMapper;
using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.Soil.GetById;

public class GetSoilByIdHandler : IRequestHandler<GetSoilByIdQuery, Result<SoilDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    
    public GetSoilByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }
    
    public async Task<Result<SoilDto>> Handle(GetSoilByIdQuery request, CancellationToken cancellationToken)
    {
        var soil = await _repositoryWrapper.SoilRepository.GetFirstOrDefaultAsync(s => s.Id == request.Id);
        
        if (soil is null)
        {
            var errorMsg = $"Не вдалося отримати тип ґрунту з Id = {request.Id}";
            return Result.Fail(new Error(errorMsg));
        }
        
        return Result.Ok(_mapper.Map<SoilDto>(soil));
    }
}