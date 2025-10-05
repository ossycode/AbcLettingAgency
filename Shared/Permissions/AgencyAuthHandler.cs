using AbcLettingAgency.Data;
using AbcLettingAgency.Enums;
using AbcLettingAgency.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Shared.Permissions;


public sealed record AgencyMemberRequirement : IAuthorizationRequirement;
public sealed record AgencyRoleRequirement(Role Role) : IAuthorizationRequirement;

public sealed class AgencyAuthHandler :
    AuthorizationHandler<IAuthorizationRequirement>
{
    private readonly ICurrentUser _current;
    private readonly AppDbContext _db;
    public AgencyAuthHandler(ICurrentUser current, AppDbContext db) => (_current, _db) = (current, db);

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext ctx, IAuthorizationRequirement req)
    {
        var uid = _current.UserId; var agencyId = _current.AgencyIdClaim;
        if (uid is null || agencyId is null) return;

        var q = _db.AgencyUsers.Where(x => x.UserId == uid && x.AgencyId == agencyId && x.IsActive);
        if (req is AgencyMemberRequirement && await q.AnyAsync()) ctx.Succeed(req);
        if (req is AgencyRoleRequirement rr && await q.AnyAsync(x => x.Role == rr.Role)) ctx.Succeed(req);
    }
}

//[Authorize(Policy = "AgencyMember")]