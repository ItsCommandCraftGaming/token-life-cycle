using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TokenLifecycle.Domain.Abstractions;
using TokenLifecycle.Domain.Enums;
using TokenLifecycle.Domain.Models;

namespace TokenLifecycle.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtConfig _jwtConfig;
        private readonly SymmetricSecurityKey _key;

        public UserService(IUserRepository userRepository, JwtConfig jwtConfig)
        {
            this._userRepository = userRepository;
            this._jwtConfig = jwtConfig;
            this._key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey));
        }

        public async Task<User?> LoginUserAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await this._userRepository.FindByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                return null;
            }
            string hashedPassword = this.Hash(password + user.Salt);

            if (hashedPassword.Equals(user.Password))
            {
                return user;
            }
            return null;
        }

        public string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        public void SimulateHashing(string password)
        {
            string dummySalt = "AAAAAAAAAAAAAAAA";
            this.Hash(password + dummySalt);
        }

        private string GenerateToken(IEnumerable<Claim> claim, TimeSpan expiration)
        {
            var allClaims = claim.ToList();
            allClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: this._jwtConfig.Issuer,
                audience: this._jwtConfig.Audience,
                claims: allClaims,
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string GenerateRefreshToken(string userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("token_type", "refresh"),
            };

            return this.GenerateToken(claims, TimeSpan.FromDays(this._jwtConfig.RefreshTokenLifetimeDays));
        }

        public string GenerateAccesToken(string userId, string email, EUserRole role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim("token_type", "acces")
            };

            return this.GenerateToken(claims, TimeSpan.FromMinutes(this._jwtConfig.AccesTokenLifetimeMinutes));
        }

        public async Task<bool> BlackListAndReplaceTokensAsync(
            User user,
            string blacklistedAcces,
            string blacklistedRefresh,
            string newAccess,
            string newRefresh,
            CancellationToken cancellationToken)
        {
            user.BlackListedTokens ??= new List<BlackListedToken>();

            if (!string.IsNullOrWhiteSpace(blacklistedAcces))
            {
                user.BlackListedTokens.Add(
                    new BlackListedToken
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Value = blacklistedAcces,
                        Type = ETokenType.Access,
                        BlackListedAt = DateTime.UtcNow,
                    });
            }

            if (!string.IsNullOrWhiteSpace(blacklistedRefresh))
            {
                user.BlackListedTokens.Add(
                    new BlackListedToken
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Value = blacklistedRefresh,
                        Type = ETokenType.Refresh,
                        BlackListedAt = DateTime.UtcNow,
                    });
            }

            user.AccessToken = new Token { Id = ObjectId.GenerateNewId().ToString(), Value = newAccess };
            user.RefreshToken = new Token { Id = ObjectId.GenerateNewId().ToString(), Value = newRefresh };

            return await this._userRepository.ReplaceUserAsync(user, cancellationToken);
        }

        public async Task<string> RegisterUser(string email, string password, EUserRole role, CancellationToken cancellationToken)
        {
            // 1. Verificăm dacă utilizatorul există deja
            if (await _userRepository.UserExists(email, cancellationToken))
            {
                throw new InvalidOperationException("User already exists.");
            }

            string salt = GenerateSaltBase64();
            string hashPassword = this.Hash(password + salt);
            return await this._userRepository.InsertAsync(new Domain.DTOs.UserDTO(email, hashPassword, salt, role), cancellationToken);
        }

        private string GenerateSaltBase64(int size = 16)
        {
            var salt = RandomNumberGenerator.GetBytes(size);
            return Convert.ToBase64String(salt);
        }

        private string Hash(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }

    }
}
