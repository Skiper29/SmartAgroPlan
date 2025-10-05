using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;
using SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.Create;
using SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.Delete;
using SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.GetAll;
using SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.GetById;

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

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FieldConditionDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(int id)
    {
        return HandleResult(await Mediator.Send(new GetFieldConditionByIdQuery(id)));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FieldConditionDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] FieldConditionCreateDto newFieldCondition)
    {
        return HandleResult(await Mediator.Send(new CreateFieldConditionCommand(newFieldCondition)));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        return HandleResult(await Mediator.Send(new DeleteFieldConditionCommand(id)));
    }
}
