using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Services;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user and returns JWT and refresh tokens.
    /// </summary>
    /// <param name="request">The login request containing the username and password.</param>
    /// <returns>An <see cref="AuthResponse"/> containing the access and refresh tokens, or Unauthorized if authentication fails.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        if (result == null)
        {
            return Unauthorized("Wrong permissions.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request containing the username and password.</param>
    /// <returns>Ok if registration is successful, BadRequest if the username already exists.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result)
        {
            return BadRequest("Username already exists");
        }

        return Ok("User created successfully");
    }

    /// <summary>
    /// Refreshes the JWT token using a valid refresh token.
    /// </summary>
    /// <param name="request">The refresh token request containing the refresh token.</param>
    /// <returns>An <see cref="AuthResponse"/> containing the new access and refresh tokens, or Unauthorized if the refresh token is invalid or expired.</returns>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        if (result == null)
        {
            return Unauthorized("Refresh Token invalid or expired.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Logs out the user by invalidating the provided refresh token.
    /// </summary>
    /// <param name="request">The logout request containing the refresh token to be invalidated.</param>
    /// <returns>Ok if logout is successful, BadRequest if the refresh token is invalid.</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var result = await _authService.LogoutAsync(request);
        if (!result)
        {
            return BadRequest("Invalid Refresh Token.");
        }

        return Ok("Successfully logged out.");
    }
}
