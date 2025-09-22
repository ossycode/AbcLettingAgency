using System.Collections.ObjectModel;

namespace AbcLettingAgency.Authorization;

public static class AppRoles
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);
    public const string Manager = nameof(Manager);

    public static IReadOnlyList<string> DefaultRoles { get; }
        = new ReadOnlyCollection<string>(new[]
        {
                Admin,
                Basic,
                Manager,
        });
    public static bool IsDefault(string roleName)
        => DefaultRoles.Any(r => r == roleName);
}
