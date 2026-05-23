using DevFastTrack.API.DTOs.Auth;
using DevFastTrack.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("external-login")]
    public async Task<IActionResult> ExternalLogin(ExternalLoginRequest request)
    {
        var result = await _authService.ExternalLoginAsync(request);
        return Ok(result);
    }
}