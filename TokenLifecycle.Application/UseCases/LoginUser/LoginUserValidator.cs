using FluentValidation;

namespace TokenLifecycle.Application.UseCases.LoginUser
{
    public class LoginUserValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserValidator()
        {
            this.RuleFor(u => u.Email).NotEmpty().WithMessage("Invalid email");
            this.RuleFor(u => u.Password).NotEmpty().WithMessage("Invalid password");
        }
    }
}
