using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.MediatR.Fields.Soil.GetAll;
using SmartAgroPlan.BLL.MediatR.Fields.Soil.GetById;

namespace SmartAgroPlan.WebAPI.Controllers.Fields.Soil;

public class SoilController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllSoilsQuery()));
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return HandleResult(await Mediator.Send(new GetSoilByIdQuery(id)));
    }
}