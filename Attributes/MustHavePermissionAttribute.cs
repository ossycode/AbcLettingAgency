using AbcLettingAgency.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace AbcLettingAgency.Attributes;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string feature, string action)
        => Policy = AppPermission.NameFor(feature, action);
}
