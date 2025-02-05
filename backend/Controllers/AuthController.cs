using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ProjectDbContext _context;

    public AuthController(IConfiguration config, ProjectDbContext context)
    {
        _config = config;
        _context = context;
    }

    /// <summary>
    /// Authenticates a user based on the provided login request.
    /// </summary>
    /// <param name="request">The login request containing the username and password.</param>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> indicating the result of the authentication attempt.
    /// If the username or password is incorrect, returns an Unauthorized result with a message.
    /// If the authentication is successful, returns an Ok result with a generated JWT token.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

        if (dbUser == null || !VerifyPassword(request.Password, dbUser.PasswordHash))
        {
            return Unauthorized("Wrong permissions.");
        }

        var accessToken = GenerateJwtToken(dbUser.Username);
        var refreshToken = GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            Username = dbUser.Username,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            Device = Request.Headers["User-Agent"].ToString()
        });

        await _context.SaveChangesAsync();
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    /// <summary>
    /// Registers a new user with the provided registration details.
    /// </summary>
    /// <param name="request">The registration request containing the username and password.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the result of the registration process.
    /// Returns <see cref="BadRequest"/> if the username is already taken.
    /// Returns <see cref="Ok"/> if the registration is successful.
    /// </returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return BadRequest("Username already exists");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User created successfully");
    }

    /// <summary>
    /// Handles the refresh token request to generate a new access token and refresh token.
    /// </summary>
    /// <param name="request">The refresh token request containing the current refresh token.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the new access token and refresh token if the provided refresh token is valid and not expired.
    /// Returns <see cref="UnauthorizedResult"/> if the provided refresh token is invalid or expired.
    /// </returns>
    /// <response code="200">Returns the new access token and refresh token.</response>
    /// <response code="401">If the provided refresh token is invalid or expired.</response>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
    {
        var storedToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
        {
            return Unauthorized("Refresh Token invalid or expired.");
        }

        var newAccessToken = GenerateJwtToken(storedToken.Username);
        var newRefreshToken = GenerateRefreshToken();

        _context.RefreshTokens.Remove(storedToken);
        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            Username = storedToken.Username,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        });

        await _context.SaveChangesAsync();

        return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }

    /// Logs out the user by invalidating the provided refresh token.
    /// </summary>
    /// <param name="request">The logout request containing the refresh token to be invalidated.</param>
    /// <returns>
    /// A <see cref="IActionResult"/> indicating the result of the logout operation.
    /// Returns <see cref="BadRequest"/> if the provided refresh token is invalid.
    /// Returns <see cref="Ok"/> if the logout operation is successful.
    /// </returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var storedToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken == null)
        {
            return BadRequest("Invalid Refresh Token.");
        }

        _context.RefreshTokens.Remove(storedToken);
        await _context.SaveChangesAsync();

        return Ok("Successfully logged out.");
    }


    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: new List<Claim> { new Claim(ClaimTypes.Name, username) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private bool VerifyPassword(string enteredPassword, string storedHash)
    {
        var enteredHash = HashPassword(enteredPassword);
        return enteredHash == storedHash;
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

}
