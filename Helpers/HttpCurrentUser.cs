using AbcLettingAgency.Authorization;
using AbcLettingAgency.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Npgsql;
using System.Security.Claims;

namespace AbcLettingAgency.Helpers;

public interface ICurrentUser
{
    Guid? UserId { get; }
    long? AgencyIdClaim {  get; }
    Task<long> GetAgencyId();

    bool IsPlatform();
    Task<long> RequireAgencyId();
}

public class HttpCurrentUser(IHttpContextAccessor http, HybridCache cache, NpgsqlDataSource dataSource)
    : ICurrentUser
{
    public Guid? UserId
        => Guid.TryParse(http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
           ? id : null;

    public long? AgencyIdClaim =>
    long.TryParse(
        http.HttpContext?.User?.FindFirstValue(AppClaim.AgencyId), 
        out var id
    ) ? id : null;

    public async Task<long> GetAgencyId()
    {
        if(AgencyIdClaim is long idFromClaim) return idFromClaim;

        var userId = UserId ?? throw new UnauthorizedAccessException("No user");

        return await cache.GetOrCreateAsync(CacheKeys.UserAgency(userId), async ct =>
        {
            //var agencyId = await db.AgencyUsers
            //.Where(u => u.UserId == userId && u.IsActive)
            //.OrderBy(u => u.JoinedAt)
            //.Select(u =>  u.AgencyId )
            //.FirstOrDefaultAsync(ct);

            //if (agencyId == 0) throw new InvalidOperationException("User has not agency membership");

            //return agencyId;
            await using var conn = await dataSource.OpenConnectionAsync(ct);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                    select "AgencyId"
                    from "AgencyUsers"
                    where "UserId" = @uid and "IsActive" = true
                    order by "JoinedAt"
                    limit 1
                    """;
            cmd.Parameters.AddWithValue("uid", userId);
            var result = await cmd.ExecuteScalarAsync(ct);
            if (result is null) throw new InvalidOperationException("User has no agency membership");
            return (long)result;
        },
        new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5)
        });
    }

    public bool IsPlatform()
    {
        var u = http.HttpContext?.User;
        if (u?.Identity?.IsAuthenticated != true) return true;
        return u.HasClaim(AppClaim.PlatformRole, PlatformRoles.Owner)
            || u.HasClaim(AppClaim.PlatformRole, PlatformRoles.Admin)
            || u.HasClaim(AppClaim.PlatformRole, PlatformRoles.Support);
    }
    public async Task<long> RequireAgencyId()
    => AgencyIdClaim ?? await GetAgencyId();
}