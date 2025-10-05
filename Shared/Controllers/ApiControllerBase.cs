using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Exceptions;
using AbcLettingAgency.Shared.Paging;
using AbcLettingAgency.Shared.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AbcLettingAgency.Shared.Controllers;

[Authorize]
[Route("api/[controller]")]
public abstract class ApiControllerBase<TEntity, TDto>(IEntityServiceFactory factory) : BaseController
    where TEntity : class, IEntityBase
{
    protected IEntityService<TEntity> EntityService { get; } = factory.For<TEntity>();

    protected abstract Expression<Func<TEntity, TDto>> Selector { get; }

    [NonAction]
    protected async Task<ActionResult<IEnumerable<TDto>>> ListAsync(
        IQueryable<TEntity>? source = null,
        CancellationToken ct = default)
    {
        source ??= EntityService.GetAll(); 
        var list = await source.Select(Selector).ToListAsync(ct);
        return Ok(list);
    }

    [NonAction]
    protected async Task<ActionResult<PagedList<TDto>>> PagedAsync(
        QueryOptions opts,
        IEnumerable<FilterRule>? filters = null,
        IQueryable<TEntity>? source = null,
        CancellationToken ct = default)
    {
        source ??= EntityService.GetAll();
        var page = await source.SelectPageAsync(Selector, opts, filters, ct);
        return Ok(page);
    }

    [NonAction]
    protected async Task<ActionResult<TDto>> SingleAsync(long id, CancellationToken ct = default)
    {
        var dto = await EntityService.GetByIdAsync(id, Selector, ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}
