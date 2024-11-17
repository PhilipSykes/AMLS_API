using MongoDB.Driver;

namespace Common.Database.Interfaces;

public interface IDatabaseConnection
{
    IMongoDatabase Database { get; }
    Task<bool> HealthCheckAsync();
}