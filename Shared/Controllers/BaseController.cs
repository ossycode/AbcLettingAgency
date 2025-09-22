using AbcLettingAgency.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AbcLettingAgency.Shared.Controllers;

[ApiController]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status204NoContent)]

[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public abstract class BaseController : ControllerBase
{
    protected IActionResult FromError(AppError error)
    {
        var status = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };

        var factory = HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        var pd = factory.CreateProblemDetails(
            HttpContext,
            statusCode: status,
            title: error.Message,
            type: null,
            detail: null,
            instance: HttpContext.Request.Path
        );

        if (error.Details is not null && error.Details.Any())
        {
            pd.Extensions["errors"] = error.Details;
        }

        pd.Extensions["traceId"] = HttpContext.TraceIdentifier;

        return StatusCode(status, pd);
    }

    protected IActionResult FromResult(Result r)
    => r.IsSuccess ? NoContent() : FromError(r.Errors[0]);

    protected IActionResult FromResult<T>(Result<T> r)
        => r.IsSuccess ? Ok(r.Value) : FromError(r.Errors[0]);
}


