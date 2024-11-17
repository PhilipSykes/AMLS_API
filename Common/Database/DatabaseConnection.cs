using Common.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
namespace Common.Database
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly MongoClient _connection;
        public IMongoDatabase Database { get; }

        public DatabaseConnection(IOptions<MongoDBConfig> options)
        {
            var config = options.Value;
            _connection = new MongoClient(config.ConnectionString);
            Database = _connection.GetDatabase(config.DatabaseName);
        }

        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                await Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
 