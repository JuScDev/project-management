using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Repositories
{
    /// <inheritdoc/>
    public class AuthRepository : IAuthRepository
    {
        private readonly ProjectDbContext _context;

        /// <inheritdoc/>
        public AuthRepository(ProjectDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        /// <inheritdoc/>
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        /// <inheritdoc/>
        public async Task RemoveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
