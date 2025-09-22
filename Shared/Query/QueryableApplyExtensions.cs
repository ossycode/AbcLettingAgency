using AbcLettingAgency.Shared.Paging;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AbcLettingAgency.Shared.Query;

public static class QueryableApplyExtensions
{
    // Simple dynamic filter rules
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> q, IEnumerable<FilterRule>? filters)
    {
        if (filters is null) return q;

        foreach (var f in filters)
        {
            var param = Expression.Parameter(typeof(T), "e");
            var member = Expression.PropertyOrField(param, f.Field);
            Expression body;

            // Only minimal ops to keep it robust
            switch (f.Op.ToLowerInvariant())
            {
                case "contains":
                    body = Expression.Call(
                        Expression.Call(member, nameof(string.ToLower), Type.EmptyTypes),
                        nameof(string.Contains),
                        Type.EmptyTypes,
                        Expression.Constant(f.Value.ToLower()));
                    break;

                case "eq":
                    body = Expression.Equal(member, ConstantFor(member.Type, f.Value));
                    break;

                case "gt":
                    body = Expression.GreaterThan(member, ConstantFor(member.Type, f.Value));
                    break;

                case "lt":
                    body = Expression.LessThan(member, ConstantFor(member.Type, f.Value));
                    break;

                default:
                    continue; // skip unknown
            }

            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            q = q.Where(lambda);
        }

        return q;

        static Expression ConstantFor(Type t, string v)
        {
            object? parsed = v;
            if (t == typeof(int) && int.TryParse(v, out var i)) parsed = i;
            else if (t == typeof(decimal) && decimal.TryParse(v, out var d)) parsed = d;
            else if (t == typeof(DateTime) && DateTime.TryParse(v, out var dt)) parsed = dt;
            else if (t == typeof(bool) && bool.TryParse(v, out var b)) parsed = b;
            return Expression.Constant(parsed, t);
        }
    }

    // Sort by property name
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> q, string? sortBy, bool desc)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return q;

        var param = Expression.Parameter(typeof(T), "e");
        var body = Expression.PropertyOrField(param, sortBy);
        var key = Expression.Lambda(body, param);

        var method = desc ? "OrderByDescending" : "OrderBy";
        return (IQueryable<T>)typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == method && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), body.Type)
            .Invoke(null, new object[] { q, key })!;
    }

    // Free text search across selected fields (string only)
    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> q, string? term, string[] fields)
    {
        if (string.IsNullOrWhiteSpace(term) || fields.Length == 0) return q;

        var t = term.ToLowerInvariant();
        Expression? or = null;
        var p = Expression.Parameter(typeof(T), "e");

        foreach (var f in fields)
        {
            var member = Expression.PropertyOrField(p, f);
            if (member.Type != typeof(string)) continue;

            var call = Expression.Call(
                Expression.Call(member, nameof(string.ToLower), Type.EmptyTypes),
                nameof(string.Contains),
                Type.EmptyTypes,
                Expression.Constant(t));

            or = or is null ? call : Expression.OrElse(or, call);
        }

        return or is null ? q : q.Where(Expression.Lambda<Func<T, bool>>(or, p));
    }

    public static async Task<PagedList<TResult>> SelectPageAsync<T, TResult>(
        this IQueryable<T> q,
        Expression<Func<T, TResult>> selector,
        QueryOptions opts,
        IEnumerable<FilterRule>? filters = null,
        CancellationToken ct = default)
    {
        q = q.ApplySearch(opts.Search, opts.SearchFields)
             .ApplyFilters(filters)
             .ApplySorting(opts.SortBy, opts.SortDesc);

        var total = await q.CountAsync(ct);

        var page = Math.Max(1, opts.Page);
        var size = Math.Max(1, opts.PageSize);

        var items = await q.Skip((page - 1) * size)
                           .Take(size)
                           .Select(selector)
                           .ToListAsync(ct);

        return new PagedList<TResult>
        {
            Items = items,
            Page = page,
            PageSize = size,
            Total = total
        };
    }
}