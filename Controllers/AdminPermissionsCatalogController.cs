using AbcLettingAgency.Attributes;
using AbcLettingAgency.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbcLettingAgency.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminPermissionsCatalogController : ControllerBase
{
    [HttpGet]
    [HasPermission(AppFeature.RoleClaims, AppAction.Read)]
    public IActionResult GetAll()
    {
        var items = AppPermissions.AllPermissions
            .OrderBy(p => p.Feature).ThenBy(p => p.Action)
            .Select(p => new
            {
                p.Name,
                p.Feature,
                p.Action,
                p.Group,
                p.Description,
                p.IsBasic
            });
        return Ok(items);
    }
}
