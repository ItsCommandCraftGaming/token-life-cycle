using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenLifecycle.Domain.Enums;

namespace TokenLifecycle.Domain.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public EUserRole Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public Token AccessToken { get; set; }
        public Token RefreshToken { get; set; }
        public List<BlackListedToken> BlackListedTokens { get; set; } = new List<BlackListedToken>();
    }
}
