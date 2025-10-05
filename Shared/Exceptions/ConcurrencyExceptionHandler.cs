using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Shared.Exceptions;

public sealed class ConcurrencyExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
    {
        if (exception is not DbUpdateConcurrencyException cex) return false;

        var conflicts = cex.Entries.Select(e => new {
            Entity = e.Metadata.ClrType.Name,
            State = e.State.ToString(),
            Keys = e.Properties.Where(p => p.Metadata.IsPrimaryKey()).ToDictionary(
                p => p.Metadata.Name, p => e.Property(p.Metadata.Name).CurrentValue)
        });

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Concurrency conflict",
            Detail = "Your changes were rejected because the record was modified by someone else. Reload and try again."
        };
        problem.Extensions["conflicts"] = conflicts;

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        await httpContext.Response.WriteAsJsonAsync(problem, token);
        return true;
    }
}
