using TokenLifecycle.Domain.Enums;
using TokenLifecycle.Domain.Models;

namespace TokenLifecycle.Domain.Abstractions
{
    public interface IUserService
    {
        Task<string> RegisterUser(string email, string password, EUserRole role, CancellationToken cancellationToken);

        Task<User?> LoginUserAsync(string email, string password, CancellationToken cancellationToken);

        string GenerateAccesToken(string userId, string email, EUserRole role);

        string GenerateRefreshToken(string userId);

        Task<bool> BlackListAndReplaceTokensAsync(
            User user,
            string blacklistedAcces,
            string blacklistedRefresh,
            string newAccess,
            string newRefresh,
            CancellationToken cancellationToken);

        void SimulateHashing(string password);

        string GetUserIdFromToken(string token);

    }
}
