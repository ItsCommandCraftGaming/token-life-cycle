using TokenLifecycle.Domain.Enums;

namespace TokenLifecycle.Domain.Models
{
    public class BlackListedToken : Token
    {
        public ETokenType Type { get; set; }

        public DateTime? BlackListedAt { get; set; } = null;
    }
}
