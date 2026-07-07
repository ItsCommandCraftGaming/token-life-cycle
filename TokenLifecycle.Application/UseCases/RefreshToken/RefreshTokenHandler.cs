using MediatR;
using TokenLifecycle.Domain.Abstractions;

namespace TokenLifecycle.Application.UseCases.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public RefreshTokenHandler(IUserService userService, IUserRepository userRepository)
        {
            this._userService = userService;
            this._userRepository = userRepository;
        }
        public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var isBlackListed = await this._userRepository.IsTokenBlacklistedAsync(request.RefreshToken, cancellationToken);

            if (isBlackListed)
            {
                return new RefreshTokenResponse
                {
                    AccesToken = string.Empty,
                    Message = "Token is blacklisted"
                };
            }

            var userId = this._userService.GetUserIdFromToken(request.RefreshToken);
            if (userId == null)
            {
                return new RefreshTokenResponse
                {
                    AccesToken = string.Empty,
                    Message = "Invalid token"
                };
            }

            var user = await this._userRepository.FindByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return new RefreshTokenResponse
                {
                    AccesToken = string.Empty,
                    Message = "User not found"
                };
            }

            var accesToken = this._userService.GenerateAccesToken(user.Id, user.Email, user.Role);
            var refreshToken = this._userService.GenerateRefreshToken(user.Id);

            await this._userService.BlackListAndReplaceTokensAsync(
                user,
                user.AccessToken?.Value ?? string.Empty,
                user.RefreshToken?.Value ?? string.Empty,
                accesToken,
                refreshToken,
                cancellationToken);

            return new RefreshTokenResponse
            {
                AccesToken = accesToken,
                RefreshToken = refreshToken,
                Message = "Token refreshed"
            };
        }
    }
}
