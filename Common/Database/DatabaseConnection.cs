using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.Database;

public interface IDatabaseConnection
{
    IMongoDatabase Database { get; }
    Task<bool> HealthCheckAsync();
}

public class DatabaseConnection : IDatabaseConnection
{
    private readonly MongoClient _connection;

    public DatabaseConnection(IOptions<MongoDBConfig> options)
    {
        var config = options.Value;

        if (string.IsNullOrEmpty(config?.ConnectionString))
            throw new ArgumentException(
                "MongoDB ConnectionString is null or empty. Check your configuration.");

        Console.WriteLine($"Attempting to connect to MongoDB with connection string: " +
                          $"{config.ConnectionString.Substring(0, 20)}... " +
                          $"and database: {config.DatabaseName}");

        _connection = new MongoClient(config.ConnectionString);
        Database = _connection.GetDatabase(config.DatabaseName);
    }

    public IMongoDatabase Database { get; }

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