using FluentValidation;

namespace TokenLifecycle.Application.UseCases.RegisterUser
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserValidator()
        {
            //this.RuleFor(r => r.Email).EmailAddress().WithMessage("Adresa de email nu este validă.");
            //this.RuleFor(r => r.Password).Custom(ValidPassword);
        }

        private void ValidPassword(string password, ValidationContext<RegisterUserRequest> context)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                context.AddFailure("Parola este obligatorie.");
                return;
            }
            if (password.Length < 6)
            {
                context.AddFailure("Parola trebuie să aibă cel puțin 6 caractere.");
            }
            if (!password.Any(char.IsUpper))
            {
                context.AddFailure("Parola trebuie să conțină cel puțin o literă mare.");
            }
            if (!password.Any(char.IsLower))
            {
                context.AddFailure("Parola trebuie să conțină cel puțin o literă mică.");
            }
            if (!password.Any(char.IsDigit))
            {
                context.AddFailure("Parola trebuie să conțină cel puțin o cifră.");
            }
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                context.AddFailure("Parola trebuie să conțină cel puțin un caracter special.");
            }
        }
    }
}
