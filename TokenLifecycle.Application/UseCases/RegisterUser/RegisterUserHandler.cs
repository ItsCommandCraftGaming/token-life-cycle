using FluentValidation;
using MediatR;
using TokenLifecycle.Domain.Abstractions;

namespace TokenLifecycle.Application.UseCases.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, RegisterUserResponse>
    {
        private readonly IUserService _userService;
        private readonly IValidator<RegisterUserRequest> _validator;

        public RegisterUserHandler(IUserService userService, IValidator<RegisterUserRequest> validator)
        {
            _userService = userService;
            _validator = validator;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            // 1. Validăm cererea folosind validatorul injectat
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                // Concatenăm toate mesajele de eroare de validare
                string errors = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
                return new RegisterUserResponse
                {
                    Ok = false,
                    Message = errors,
                    UserId = null
                };
            }

            // 2. Dacă validarea a trecut, apelăm serviciul pentru salvare
            string userId = await _userService.RegisterUser(request.Email, request.Password, request.Role, cancellationToken);
            return new RegisterUserResponse(userId);
        }
    }
}
