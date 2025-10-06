using AbcLettingAgency.Abstracts;
using AbcLettingAgency.Dtos;
using AbcLettingAgency.Services;
using AbcLettingAgency.Shared.Controllers;
using AbcLettingAgency.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbcLettingAgency.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService accountService) : BaseController
{
    private readonly IAuthService _accountService = accountService;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        return FromResult(await _accountService.CreateUserAsync(request));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _accountService.LoginAsync(request);

        if (!result.IsSuccess)
            return Unauthorized(result.Errors[0]);

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["REFRESH_TOKEN"];

        return FromResult(await _accountService.RefreshTokenAsync(refreshToken));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return FromResult(await _accountService.LogUserOut(User));
    }
}
