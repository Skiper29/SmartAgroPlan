using AutoMapper;
using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Crops.Create;

public class CreateCropHandler : IRequestHandler<CreateCropCommand, Result<CropVarietyDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateCropHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<CropVarietyDto>> Handle(CreateCropCommand request, CancellationToken cancellationToken)
    {
        var cropEntity = _mapper.Map<CropVariety>(request.CropVariety);
        if (cropEntity is null)
        {
            const string errorMsg = "Не вдалося відобразити DTO в сутність сорту культури";
            return Result.Fail(new Error(errorMsg));
        }

        var createdCrop = await _repositoryWrapper.CropVarietyRepository.CreateAsync(cropEntity);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            const string errorMsg = "Не вдалося створити сорт культури";
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<CropVarietyDto>(createdCrop));
    }
}
