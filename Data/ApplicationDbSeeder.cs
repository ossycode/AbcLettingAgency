using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Data;

public class ApplicationDbSeeder(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager,
    AppDbContext dbContext)
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly AppDbContext _dbContext = dbContext;

    public async Task SeedDatabaseAsync()
    {
        await CheckAndApplyPendingMigrationAsync();
        await SeedRolesAsync();

        await SeedBasicUserAsync();

        await SeedAdminUserAsync();
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

        var admin = await _userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new AppUser
            {
                FirstName = "Ossy",
                LastName = "Labs",
                Email = email,
                UserName = userName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };

            var create = await _userManager.CreateAsync(admin, AppCredentials.Password);
            if (!create.Succeeded)
                throw new InvalidOperationException(string.Join("; ", create.Errors.Select(e => e.Description)));

            if (string.IsNullOrEmpty(admin.SecurityStamp))
                await _userManager.UpdateSecurityStampAsync(admin);

            admin = await _userManager.FindByEmailAsync(email) ?? throw new InvalidOperationException("Admin user not found after creation.");
        }

        var desiredRoles = new[] { AppRoles.Admin, AppRoles.Basic };
        foreach (var role in desiredRoles)
        {
            if (!await _userManager.IsInRoleAsync(admin, role))
            {
                var add = await _userManager.AddToRoleAsync(admin, role);
                if (!add.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", add.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task SeedBasicUserAsync()
    {
        var basicUser = new AppUser
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "johnd@abc.com",
            UserName = "johnd",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            NormalizedEmail = "JOHND@LETTINGAGENCY.COM",
            NormalizedUserName = "JOHND",
            IsActive = true
        };

        if (!await _userManager.Users.AnyAsync(u => u.Email == "johnd@lettingagency.com"))
        {
            var password = new PasswordHasher<AppUser>();
            basicUser.PasswordHash = password.HashPassword(basicUser, AppCredentials.Password);
            await _userManager.CreateAsync(basicUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(basicUser, AppRoles.Basic))
        {
            await _userManager.AddToRoleAsync(basicUser, AppRoles.Basic);
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
        var current = await _roleManager.GetClaimsAsync(role);
        foreach (var p in permissions)
        {
            if (!current.Any(c => c.Type == AppClaim.Permission && c.Value == p.Name))
            {
                await _dbContext.RoleClaims.AddAsync(new IdentityRoleClaim<Guid>
                {
                    RoleId = role.Id,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = p.Name
                });
            }
        }
        await _dbContext.SaveChangesAsync();
    }
}
