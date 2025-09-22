using System.Collections.ObjectModel;


namespace AbcLettingAgency.Authorization;


public record AppPermission(string Feature, string Action, string Group, string Description, bool IsBasic = false)
{
    public string Name => NameFor(Feature, Action);

    public static string NameFor(string feature, string action)
    {
        return $"Permissions.{feature}.{action}";
    }
}

public class AppPermissions
{
    private static readonly AppPermission[] _all =
    [
            new(AppFeature.Users, AppAction.Create, AppRoleGroup.SystemAccess, "Create Users"),
            new(AppFeature.Users, AppAction.Update, AppRoleGroup.SystemAccess, "Update Users"),
            new(AppFeature.Users, AppAction.Read, AppRoleGroup.SystemAccess, "Read Users"),
            new(AppFeature.Users, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Users"),

            new(AppFeature.UserRoles, AppAction.Read, AppRoleGroup.SystemAccess, "Read User Roles"),
            new(AppFeature.UserRoles, AppAction.Update, AppRoleGroup.SystemAccess, "Update User Roles"),

            new(AppFeature.Roles, AppAction.Read, AppRoleGroup.SystemAccess, "Read Roles"),
            new(AppFeature.Roles, AppAction.Create, AppRoleGroup.SystemAccess, "Create Roles"),
            new(AppFeature.Roles, AppAction.Update, AppRoleGroup.SystemAccess, "Update Roles"),
            new(AppFeature.Roles, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Roles"),

            new(AppFeature.RoleClaims, AppAction.Read, AppRoleGroup.SystemAccess, "Read Role Claims/Permissions"),
            new(AppFeature.RoleClaims, AppAction.Update, AppRoleGroup.SystemAccess, "Update Role Claims/Permissions"),

            new(AppFeature.Properties, AppAction.Read, AppRoleGroup.ManagementHierarchy, "Read Properties", IsBasic: true),
            new(AppFeature.Properties, AppAction.Create, AppRoleGroup.ManagementHierarchy, "Create Properties"),
            new(AppFeature.Properties, AppAction.Update, AppRoleGroup.ManagementHierarchy, "Update Properties"),
            new(AppFeature.Properties, AppAction.Delete, AppRoleGroup.ManagementHierarchy, "Delete Properties"),

            new(AppFeature.Entity, AppAction.Read, AppRoleGroup.ManagementHierarchy, "Read Entities", IsBasic: true),
            new(AppFeature.Entity, AppAction.Create, AppRoleGroup.ManagementHierarchy, "Create Entities"),
            new(AppFeature.Entity, AppAction.Update, AppRoleGroup.ManagementHierarchy, "Update Entities"),
            new(AppFeature.Entity, AppAction.Delete, AppRoleGroup.ManagementHierarchy, "Delete Entities")
    ];

    public static IReadOnlyList<AppPermission> AdminPermissions { get; } =
        new ReadOnlyCollection<AppPermission>(_all.Where(p => !p.IsBasic).ToArray());

    public static IReadOnlyList<AppPermission> BasicPermissions { get; } =
        new ReadOnlyCollection<AppPermission>(_all.Where(p => p.IsBasic).ToArray());

    public static IReadOnlyList<AppPermission> AllPermissions { get; } =
        new ReadOnlyCollection<AppPermission>(_all);

    public static IEnumerable<AppPermission> ByFeature(string feature) => _all.Where(p => p.Feature == feature);
    public static IEnumerable<AppPermission> ByGroup(string group) => _all.Where(p => p.Group == group);
}