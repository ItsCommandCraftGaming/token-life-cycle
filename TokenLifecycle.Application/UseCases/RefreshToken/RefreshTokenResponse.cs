using System.Text.Json.Serialization;

namespace TokenLifecycle.Application.UseCases.RefreshToken
{
    public class RefreshTokenResponse
    {
        public string AccesToken { get; set; }

        public string Message { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
