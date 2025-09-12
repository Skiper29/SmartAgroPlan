using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.MediatR.Crops.GetAll;
using SmartAgroPlan.BLL.MediatR.Crops.GetById;

namespace SmartAgroPlan.WebAPI.Controllers.Crops;

public class CropController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllCropsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return HandleResult(await Mediator.Send(new GetCropByIdQuery(id)));
    }
}
