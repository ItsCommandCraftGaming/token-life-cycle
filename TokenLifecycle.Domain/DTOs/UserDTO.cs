using TokenLifecycle.Domain.Enums;

namespace TokenLifecycle.Domain.DTOs
{
    public record UserDTO(string email, string hashedPassword, string salt, EUserRole role);

}
