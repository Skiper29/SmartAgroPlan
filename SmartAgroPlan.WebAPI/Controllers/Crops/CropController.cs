using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.MediatR.Crops.Create;
using SmartAgroPlan.BLL.MediatR.Crops.GetAll;
using SmartAgroPlan.BLL.MediatR.Crops.GetById;
using SmartAgroPlan.BLL.MediatR.Crops.Update;

namespace SmartAgroPlan.WebAPI.Controllers.Crops;

public class CropController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CropVarietyDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllCropsQuery()));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CropVarietyDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(int id)
    {
        return HandleResult(await Mediator.Send(new GetCropByIdQuery(id)));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CropVarietyDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(CropVarietyCreateDto cropVariety)
    {
        return HandleResult(await Mediator.Send(new CreateCropCommand(cropVariety)));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CropVarietyDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(CropVarietyUpdateDto cropVariety)
    {
        return HandleResult(await Mediator.Send(new UpdateCropCommand(cropVariety)));
    }
}
