using AbcLettingAgency.Abstracts;
using AbcLettingAgency.Dtos.Request;
using AbcLettingAgency.Shared.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbcLettingAgency.Controllers;

[Authorize]
[Route("api/user")]
public class UserController(IUserService userService) : BaseController
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var result = await _userService.GetMeAsync(User, ct);
        return FromResult(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateMeRequest req, CancellationToken ct)
    {
        return FromResult(await _userService.UpdateMeAsync(User, req, ct));
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        return FromResult(await _userService.ChangePasswordAsync(User, req, ct));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMe([FromBody] DeleteMeRequest req, CancellationToken ct)
    {
        return FromResult(await _userService.DeleteMeAsync(User, req, ct));
    }
}
