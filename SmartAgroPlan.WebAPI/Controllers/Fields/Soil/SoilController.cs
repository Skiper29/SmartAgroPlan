using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.MediatR.Fields.Soil.GetAll;

namespace SmartAgroPlan.WebAPI.Controllers.Fields.Soil;

public class SoilController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllSoilsQuery()));
    }
}