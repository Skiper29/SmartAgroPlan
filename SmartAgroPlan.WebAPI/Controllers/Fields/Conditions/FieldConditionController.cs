using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;
using SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.GetAll;

namespace SmartAgroPlan.WebAPI.Controllers.Fields.Conditions;

public class FieldConditionController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FieldConditionDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllFieldConditionsQuery()));
    }
}
