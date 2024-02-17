using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Data.NLimit.Common.EntitiesModels.MongoDb;

namespace Data.NLimit.Common.DataContext.MongoDb
{
    public class MongoDbContext : DbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Book> Books => _database.GetCollection<Book>("book");
    }
}
