using MediatR;
using TokenLifecycle.Domain.Abstractions;
using TokenLifecycle.Domain.Models;

namespace TokenLifecycle.Application.UseCases.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
    {
        private readonly IUserService _userService;

        public LoginUserHandler(IUserService userService)
        {
            this._userService = userService;
        }
        public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            User? user = await this._userService.LoginUserAsync(request.Email, request.Password, cancellationToken);

            if (user == null)
            {
                this._userService.SimulateHashing(request.Password);

                return new LoginUserResponse
                {
                    AccesToken = string.Empty,
                    RefreshToken = string.Empty,
                    Message = "Invalid credentials"
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

            return new LoginUserResponse
            {
                AccesToken = accesToken,
                RefreshToken = refreshToken,
                Message = "Logged in succesfully"
            };
        }
    }
}
