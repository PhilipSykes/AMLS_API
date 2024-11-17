
namespace Common;

public class RabbitMQConfig
{
    public string HostName { get; set; } = string.Empty;
    public int Port { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = string.Empty;
}

public class MongoDBConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}


