using AutoMapper;
using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.Soil.GetAll;

public class GetAllSoilsHandler : IRequestHandler<GetAllSoilsQuery,Result<IEnumerable<SoilDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    
    public GetAllSoilsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }
    
    public async Task<Result<IEnumerable<SoilDto>>> Handle(GetAllSoilsQuery request, CancellationToken cancellationToken)
    {
        var soils = await _repositoryWrapper.SoilRepository.GetAllAsync();

        if (!soils.Any())
        {
            var errorMsg = "Не вдалося отримати типи ґрунтів";
            return Result.Fail(new Error(errorMsg));
        }
        return Result.Ok(_mapper.Map<IEnumerable<SoilDto>>(soils));
    }
}