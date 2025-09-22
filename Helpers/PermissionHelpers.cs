using AbcLettingAgency.Authorization;

namespace AbcLettingAgency.Helpers
{
    public static class PermissionHelpers
    {
        public static bool IsKnownPermission(string name)
            => AppPermissions.AllPermissions.Any(p => p.Name.Equals(name, StringComparison.Ordinal));

        public static IEnumerable<string> UnknownPermissions(IEnumerable<string> names)
            => names.Where(n => !IsKnownPermission(n));
    }
}
