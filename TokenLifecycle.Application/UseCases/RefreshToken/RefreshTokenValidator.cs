using FluentValidation;

namespace TokenLifecycle.Application.UseCases.RefreshToken
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenValidator()
        {
            this.RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
