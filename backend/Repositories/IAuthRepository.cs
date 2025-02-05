using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Repositories
{
    /// <summary>
    /// Interface for authentication repository.
    /// </summary>
    public interface IAuthRepository
    {
        /// <summary>
        /// Gets the user by username asynchronously.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user with the specified username.</returns>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Adds a new user asynchronously.
        /// </summary>
        /// <param name="user">The user to add.</param>
        Task AddUserAsync(User user);

        /// <summary>
        /// Adds a new refresh token asynchronously.
        /// </summary>
        /// <param name="refreshToken">The refresh token to add.</param>
        Task AddRefreshTokenAsync(RefreshToken refreshToken);

        /// <summary>
        /// Gets the refresh token by token string asynchronously.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <returns>The refresh token with the specified token string.</returns>
        Task<RefreshToken?> GetRefreshTokenAsync(string token);

        /// <summary>
        /// Removes the refresh token asynchronously.
        /// </summary>
        /// <param name="refreshToken">The refresh token to remove.</param>
        Task RemoveRefreshTokenAsync(RefreshToken refreshToken);
    }
}
