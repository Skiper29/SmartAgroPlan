using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.BLL.MediatR.Fields.Field.GetAll;
using SmartAgroPlan.BLL.MediatR.Fields.Field.GetById;

namespace SmartAgroPlan.WebAPI.Controllers.Fields.Field;

public class FieldController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FieldDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllFieldsQuery()));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FieldDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(int id)
    {
        return HandleResult(await Mediator.Send(new GetFieldByIdQuery(id)));
    }
}
