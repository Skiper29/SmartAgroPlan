using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.MediatR.Crops.GetAll;

namespace SmartAgroPlan.WebAPI.Controllers.Crops;

public class CropController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllCropsQuery()));
    }
}
