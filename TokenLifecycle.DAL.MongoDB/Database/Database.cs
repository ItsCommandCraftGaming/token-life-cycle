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
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;

        public Database(DatabaseSettings databaseSettings)
        {
            _client = new MongoClient(databaseSettings.ConnectionString);
            _db = _client.GetDatabase(databaseSettings.DatabaseName);
        }

        //metoda publica care imi returneaza colectia dupa collection name
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _db.GetCollection<T>(collectionName);
        }

        //metoda publica care imi returneaza colectia dintr-o baza de date specifica
        public IMongoCollection<T> GetCollectionFromDatabase<T>(string databaseName, string collectionName)
        {
            return _client.GetDatabase(databaseName).GetCollection<T>(collectionName);
        }
    }
}
