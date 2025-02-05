using ProjectManagementAPI.Models;
using System.Threading.Tasks;

namespace ProjectManagementAPI.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user and generates JWT and refresh tokens.
        /// </summary>
        /// <param name="request">The login request containing the username and password.</param>
        /// <param name="ipAddress">The IP address of the client.</param>
        /// <param name="userAgent">The user agent of the client.</param>
        /// <returns>An <see cref="AuthResponse"/> containing the access and refresh tokens, or null if authentication fails.</returns>
        Task<AuthResponse?> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent);

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The registration request containing the username and password.</param>
        /// <returns>True if registration is successful, false if the username already exists.</returns>
        Task<bool> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Refreshes the JWT token using a valid refresh token.
        /// </summary>
        /// <param name="request">The refresh token request containing the refresh token.</param>
        /// <returns>An <see cref="AuthResponse"/> containing the new access and refresh tokens, or null if the refresh token is invalid or expired.</returns>
        Task<AuthResponse?> RefreshTokenAsync(RefreshRequest request);

        /// <summary>
        /// Logs out the user by invalidating the provided refresh token.
        /// </summary>
        /// <param name="request">The logout request containing the refresh token to be invalidated.</param>
        /// <returns>True if logout is successful, false if the refresh token is invalid.</returns>
        Task<bool> LogoutAsync(LogoutRequest request);
    }
}
