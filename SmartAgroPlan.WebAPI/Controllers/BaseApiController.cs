using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.DTO.Shared;

namespace SmartAgroPlan.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BaseApiController : ControllerBase
{
    private IMediator? _mediator;
    
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>() !;
    
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result.Value == null)
                return NotFound("Not Found");

            return Ok(result.Value);
        }

        if (result.HasError(error => error.Message == "Unauthorized"))
        {
            return Unauthorized();
        }

        return BadRequest(result.Errors.Select(x => new ErrorDto() { Message = x.Message }));
    }
}