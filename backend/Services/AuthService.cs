using Microsoft.IdentityModel.Tokens;
using ProjectManagementAPI.Models;
using ProjectManagementAPI.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProjectManagementAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _authRepository;

        public AuthService(IConfiguration config, IAuthRepository authRepository)
        {
            _config = config;
            _authRepository = authRepository;
        }

        /// <inheritdoc/>
        public async Task<AuthResponse?> LoginAsync(LoginRequest request, string? ipAddress, string? device)
        {
            var dbUser = await _authRepository.GetUserByUsernameAsync(request.Username);

            if (dbUser == null || !VerifyPassword(request.Password, dbUser.PasswordHash))
            {
                return null;
            }

            var accessToken = GenerateJwtToken(dbUser.Username);
            var refreshToken = GenerateRefreshToken();

            await _authRepository.AddRefreshTokenAsync(new RefreshToken
            {
                Token = refreshToken,
                Username = dbUser.Username,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IpAddress = ipAddress,
                Device = device
            });

            return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        /// <inheritdoc/>
        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (await _authRepository.GetUserByUsernameAsync(request.Username) != null)
            {
                return false;
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password)
            };

            await _authRepository.AddUserAsync(user);
            return true;
        }

        /// <inheritdoc/>
        public async Task<AuthResponse?> RefreshTokenAsync(RefreshRequest request)
        {
            var storedToken = await _authRepository.GetRefreshTokenAsync(request.RefreshToken);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return null;
            }

            var newAccessToken = GenerateJwtToken(storedToken.Username);
            var newRefreshToken = GenerateRefreshToken();

            await _authRepository.RemoveRefreshTokenAsync(storedToken);
            await _authRepository.AddRefreshTokenAsync(new RefreshToken
            {
                Token = newRefreshToken,
                Username = storedToken.Username,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });

            return new AuthResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        }

        /// <inheritdoc/>
        public async Task<bool> LogoutAsync(LogoutRequest request)
        {
            var storedToken = await _authRepository.GetRefreshTokenAsync(request.RefreshToken);

            if (storedToken == null)
            {
                return false;
            }

            await _authRepository.RemoveRefreshTokenAsync(storedToken);
            return true;
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
}
