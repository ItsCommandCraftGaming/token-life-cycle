using MediatR;
using TokenLifecycle.Domain.Enums;

namespace TokenLifecycle.Application.UseCases.RegisterUser
{
    public class RegisterUserRequest : IRequest<RegisterUserResponse>
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public EUserRole Role { get; set; }
    }
}
