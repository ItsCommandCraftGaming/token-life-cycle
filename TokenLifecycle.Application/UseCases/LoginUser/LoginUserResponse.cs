using System.Text.Json.Serialization;

namespace TokenLifecycle.Application.UseCases.LoginUser
{
    public class LoginUserResponse
    {
        public string AccesToken { get; set; }

        public string Message { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
