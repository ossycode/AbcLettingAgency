using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AbcLettingAgency.Shared.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, ProblemDetailsFactory pdf) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);

        var pd = pdf.CreateProblemDetails(httpContext,
         statusCode: StatusCodes.Status500InternalServerError,
         title: "An unexpected error occurred",
         detail: httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                     ? exception.GetBaseException().Message
                     : null);

        pd.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = pd.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(pd, cancellationToken);
        return true;
    }
}

public class BadRequestExceptionHandler(ILogger<BadRequestExceptionHandler> logger, ProblemDetailsFactory pdf) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException) return false;

        logger.LogWarning(exception, "Bad request. TraceId: {TraceId}", httpContext.TraceIdentifier);

        var pd = pdf.CreateProblemDetails(httpContext,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request");

        pd.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = pd.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(pd, cancellationToken);
        return true;
    }
}

public sealed class NoAccessExceptionHandler(ILogger<NoAccessExceptionHandler> logger,
                                             ProblemDetailsFactory pdf)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken cancellationToken)
    {
        if (ex is not UnauthorizedAccessException) return false;

        logger.LogInformation(ex, "Unauthorized. TraceId: {TraceId}", ctx.TraceIdentifier);

        var pd = pdf.CreateProblemDetails(ctx,
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Unauthorized",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#name-401-unauthorized");

        pd.Extensions["traceId"] = ctx.TraceIdentifier;

        ctx.Response.StatusCode = pd.Status!.Value;
        await ctx.Response.WriteAsJsonAsync(pd, cancellationToken);
        return true;
    }
}
public sealed class NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger,
                                             ProblemDetailsFactory pdf)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not KeyNotFoundException) return false;

        logger.LogInformation(exception, "Not found. TraceId: {TraceId}", ctx.TraceIdentifier);

        var pd = pdf.CreateProblemDetails(ctx,
            statusCode: StatusCodes.Status404NotFound,
            title: "Not Found",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#name-404-not-found");

        pd.Extensions["traceId"] = ctx.TraceIdentifier;

        ctx.Response.StatusCode = pd.Status!.Value;
        await ctx.Response.WriteAsJsonAsync(pd, cancellationToken);
        return true;
    }
}