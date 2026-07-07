using TokenLifecycle.Domain.DTOs;
using TokenLifecycle.Domain.Models;

namespace TokenLifecycle.Domain.Abstractions
{
    public interface IUserRepository
    {
        Task<string> InsertAsync(UserDTO userDto, CancellationToken cancellationToken = default);

        Task<bool> ReplaceUserAsync(User user, CancellationToken cancellationToken = default);

        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);

        Task<bool> UserExists(string email, CancellationToken cancellationToken);

        Task<bool> IsTokenBlacklistedAsync(string token, CancellationToken cancellationToken);

        Task<User> FindByIdAsync(string id, CancellationToken cancellationToken);

    }
}
