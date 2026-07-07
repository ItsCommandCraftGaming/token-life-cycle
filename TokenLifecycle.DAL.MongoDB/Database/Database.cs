using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenLifecycle.DAL.MongoDB.Database
{
    public class Database
    {
        private readonly IMongoDatabase _db;
        public Database(DatabaseSettings databaseSettings)
        {
            IMongoClient client = new MongoClient(databaseSettings.ConnectionString);
            _db = client.GetDatabase(databaseSettings.DatabaseName);
        }

        //metoda publica care imi returneaza colectia dupa collection name
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _db.GetCollection<T>(collectionName);
        }
    }
}
