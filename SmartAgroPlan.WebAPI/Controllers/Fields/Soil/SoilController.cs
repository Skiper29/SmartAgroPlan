using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.MediatR.Fields.Soil.GetAll;
using SmartAgroPlan.BLL.MediatR.Fields.Soil.GetById;
using SmartAgroPlan.BLL.MediatR.Fields.Soil.GetBySoilType;
using SmartAgroPlan.DAL.Enums;

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
    
    [HttpGet("{soilType}")]
    public async Task<IActionResult> GetBySoilType(SoilType soilType)
    {
        return HandleResult(await Mediator.Send(new GetSoilByTypeQuery(soilType)));
    }
}