using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TokenLifecycle.Application.UseCases.LoginUser;
using TokenLifecycle.Application.UseCases.RefreshToken;
using TokenLifecycle.Application.UseCases.RegisterUser;

namespace TokenLifecycle.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                RegisterUserResponse response = await this.mediator.Send(request, cancellationToken);

                if (response.UserId != null)
                {
                    return this.Ok(response);
                }
                return this.BadRequest(response);

            }
            catch (ValidationException ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser(LoginUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await this.mediator.Send(request, cancellationToken);

                if (!string.IsNullOrEmpty(response.AccesToken))
                {
                    Response.Cookies.Append("refreshToken", response.RefreshToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });

                    return this.Ok(new { response.Message, response.AccesToken });
                }

                return this.Unauthorized(new { response.Message });
            }
            catch (ValidationException ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpGet("user-restricted")]
        [Authorize]
        public async Task<IActionResult> UserRestricted(CancellationToken cancellationToken)
        {
            return this.Ok("Authorized");

        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return this.Unauthorized(new { Message = "No refresh token" });
            }

            var response = await this.mediator.Send(new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            });

            if (string.IsNullOrWhiteSpace(response.AccesToken))
            {
                return this.Unauthorized(new { response.Message });
            }

            Response.Cookies.Append("refreshToken", response.RefreshToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });

            return this.Ok(new { response.Message, response.AccesToken });

        }
    }
}
