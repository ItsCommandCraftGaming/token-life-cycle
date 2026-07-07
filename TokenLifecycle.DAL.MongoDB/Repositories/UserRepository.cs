using MongoDB.Driver;
using TokenLifecycle.Domain.Abstractions;
using TokenLifecycle.Domain.DTOs;
using TokenLifecycle.Domain.Models;

namespace TokenLifecycle.DAL.MongoDB.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> userCollection;

        private IMongoDatabase db;
        public UserRepository(Database.Database db)
        {
            this.userCollection = db.GetCollection<User>("Users");
        }

        public async Task<string> InsertAsync(UserDTO userDto, CancellationToken cancellationToken)
        {
            User user = new User
            {
                Email = userDto.email,
                Password = userDto.hashedPassword,
                Salt = userDto.salt,
                Role = userDto.role
            };
            await this.userCollection.InsertOneAsync(user, null, cancellationToken);
            return user.Id;
        }

        public async Task<bool> ReplaceUserAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
            var response = await this.userCollection.ReplaceOneAsync(filter, user, new ReplaceOptions(), cancellationToken);
            return response.ModifiedCount != 0;
        }

        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, email);
            return await this.userCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> UserExists(string email, CancellationToken cancellationToken)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, email);
            return await this.userCollection.Find(filter).AnyAsync(cancellationToken);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token, CancellationToken cancellationToken)
        {
            var user = await this.userCollection
                .Find(u => u.BlackListedTokens.Any(t => t.Value == token))
                .FirstOrDefaultAsync(cancellationToken);
            return user != null;
        }

        public async Task<User> FindByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await this.userCollection
                .Find(u => u.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
