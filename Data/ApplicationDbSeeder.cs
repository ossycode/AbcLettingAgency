using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using AbcLettingAgency.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace AbcLettingAgency.Data;

public class ApplicationDbSeeder(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager,
    AppDbContext dbContext)
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly AppDbContext _dbContext = dbContext;

    public async Task SeedDatabaseAsync()
    {
        var prev = _dbContext.BypassTenantRules;
        _dbContext.BypassTenantRules = true;
        try
        {
            await CheckAndApplyPendingMigrationAsync();
            await SeedRolesAsync();
            await SeedBasicUserAsync();
            await SeedAdminUserAsync();
            await SeedAgencyAdminAsync();
        }
        finally
        {
            _dbContext.BypassTenantRules = prev;
        }
    }

    private async Task CheckAndApplyPendingMigrationAsync()
    {
        if (_dbContext.Database.GetPendingMigrations().Any())
        {
            await _dbContext.Database.MigrateAsync();
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var email = AppCredentials.Email;
        var userName = email[..email.IndexOf('@')].ToLowerInvariant();

        var platformAdmin = await _userManager.FindByEmailAsync(email);
        if (platformAdmin is null)
        {
            platformAdmin = new AppUser
            {
                FirstName = "Ossy",
                LastName = "Labs",
                Email = email,
                UserName = userName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };

            var create = await _userManager.CreateAsync(platformAdmin, AppCredentials.Password);
            if (!create.Succeeded)
                throw new InvalidOperationException(string.Join("; ", create.Errors.Select(e => e.Description)));

            if (string.IsNullOrEmpty(platformAdmin.SecurityStamp))
                await _userManager.UpdateSecurityStampAsync(platformAdmin);

            platformAdmin = await _userManager.FindByEmailAsync(email) ?? throw new InvalidOperationException("Admin user not found after creation.");
        }

        var desiredRoles = new[] { AppRoles.Admin, AppRoles.Basic };
        foreach (var role in desiredRoles)
        {
            if (!await _userManager.IsInRoleAsync(platformAdmin, role))
            {
                var add = await _userManager.AddToRoleAsync(platformAdmin, role);
                if (!add.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", add.Errors.Select(e => e.Description)));
            }
        }
        var claims = await _userManager.GetClaimsAsync(platformAdmin);
        if (!claims.Any(c => c.Type == AppClaim.PlatformRole))
        {
            await _userManager.AddClaimAsync(
                platformAdmin, new Claim(AppClaim.PlatformRole, PlatformRoles.Owner));
        }
    }

    private async Task SeedBasicUserAsync()
    {
        const string email = "johnd@lettingagency.com";

        // ALWAYS load the existing user if present
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new AppUser
            {
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                UserName = "johnd",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };

            var create = await _userManager.CreateAsync(user, AppCredentials.Password);
            if (!create.Succeeded)
                throw new InvalidOperationException(string.Join("; ", create.Errors.Select(e => e.Description)));
        }

        // from this point on, ALWAYS use the persisted 'user' instance
        if (!await _userManager.IsInRoleAsync(user, AppRoles.Basic))
        {
            var addRole = await _userManager.AddToRoleAsync(user, AppRoles.Basic);
            if (!addRole.Succeeded)
                throw new InvalidOperationException(string.Join("; ", addRole.Errors.Select(e => e.Description)));
        }

        // Example platform claim (optional)
        var claims = await _userManager.GetClaimsAsync(user);
        if (!claims.Any(c => c.Type == AppClaim.PlatformRole && c.Value == PlatformRoles.Support))
        {
            var addClaim = await _userManager.AddClaimAsync(user, new Claim(AppClaim.PlatformRole, PlatformRoles.Support));
            if (!addClaim.Succeeded)
                throw new InvalidOperationException(string.Join("; ", addClaim.Errors.Select(e => e.Description)));
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in AppRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName)
                is not IdentityRole<Guid> role)
            {
                role = new IdentityRole<Guid>
                {
                    Name = roleName,
                };

                await _roleManager.CreateAsync(role);
            }

            if (roleName == AppRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(role, AppPermissions.AdminPermissions);
            }
            else if (roleName == AppRoles.Basic)
            {
                await AssignPermissionsToRoleAsync(role, AppPermissions.BasicPermissions);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(IdentityRole<Guid> role, IReadOnlyList<AppPermission> permissions)
    {
        var existing = (await _roleManager.GetClaimsAsync(role))
                   .Where(c => c.Type == AppClaim.Permission)
                   .Select(c => c.Value)
                   .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var p in permissions)
        {
            if (!existing.Contains(p.Name))
            {
                var res = await _roleManager.AddClaimAsync(role, new Claim(AppClaim.Permission, p.Name));
                if (!res.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedAgencyAdminAsync()
    {
        // Optional but recommended
        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            // Bypass tenant rules while seeding
            _dbContext.BypassTenantRules = true;

            try
            {
                const string defaultAgencySlug = "default-agency";
                var agency = await _dbContext.Agencies
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Slug == defaultAgencySlug);

                if (agency is null)
                {
                    agency = new Agency
                    {
                        Slug = defaultAgencySlug,
                        Name = "Default Agency",
                        Status = AgencyStatus.Active,
                        Email = "info@defaultagency.com",
                        PhoneNumber = "+44 0000 000000",
                        Address = new Address("1 Test Street", "London", "SW1A 1AA")
                    };
                    await _dbContext.Agencies.AddAsync(agency);
                    await _dbContext.SaveChangesAsync();
                }

                var config = await _dbContext.Set<AgencyConfiguration>()
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.AgencyId == agency.Id);

                if (config is null)
                {
                    config = new AgencyConfiguration
                    {
                        AgencyId = agency.Id,
                        DefaultRentDueDay = 1,
                        DefaultRentFrequency = RentFrequency.Monthly,
                        EnableArrearsEmails = true,
                        ArrearsEmailDays = 3,
                        PrimaryColorHex = "#1f2937",
                        SecondaryColorHex = "#6b7280"
                    };
                    await _dbContext.AddAsync(config);
                    await _dbContext.SaveChangesAsync();
                }

                const string agencyAdminEmail = "admin@defaultagency.com";

                var agencyAdmin = await _userManager.FindByEmailAsync(agencyAdminEmail);
                if (agencyAdmin is null)
                {
                    agencyAdmin = CreateAppUser(agencyAdminEmail, "Agency", "Admin");

                    var create = await _userManager.CreateAsync(agencyAdmin, AppCredentials.Password);
                    if (!create.Succeeded)
                        throw new InvalidOperationException(string.Join("; ", create.Errors.Select(e => e.Description)));

                    if (string.IsNullOrEmpty(agencyAdmin.SecurityStamp))
                        await _userManager.UpdateSecurityStampAsync(agencyAdmin);
                }

                var desiredRoles = new[] { AppRoles.Admin, AppRoles.Basic };
                foreach (var role in desiredRoles)
                {
                    if (!await _userManager.IsInRoleAsync(agencyAdmin, role))
                    {
                        var add = await _userManager.AddToRoleAsync(agencyAdmin, role);
                        if (!add.Succeeded)
                            throw new InvalidOperationException(string.Join("; ", add.Errors.Select(e => e.Description)));
                    }
                }

                var hasMembership = await _dbContext.AgencyUsers
                    .IgnoreQueryFilters() 
                    .AnyAsync(x => x.UserId == agencyAdmin.Id && x.AgencyId == agency.Id);

                if (!hasMembership)
                {
                    await _dbContext.AgencyUsers.AddAsync(new AgencyUser
                    {
                        AgencyId = agency.Id,
                        UserId = agencyAdmin.Id,
                        Role = Role.Admin,
                        IsActive = true
                    });
                    await _dbContext.SaveChangesAsync();
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            finally
            {
                _dbContext.BypassTenantRules = false;
            }
        });
    }

    private static AppUser CreateAppUser(string email, string firstName, string lastName)
    {
        var local = email.Split('@')[0];
        local = Regex.Replace(local, "[^a-zA-Z0-9]", "");
        if (string.IsNullOrWhiteSpace(local))
            local = "user" + Guid.NewGuid().ToString("N")[..6];

        return new AppUser
        {
            Email = email,
            UserName = local.ToLowerInvariant(),
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true,
            IsActive = true
        };
    }
}
