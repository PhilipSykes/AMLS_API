using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using static Common.Models.Shared;
using Common;
using Microsoft.Extensions.DependencyInjection;

namespace Common.MessageBroker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageBroker(this IServiceCollection services)
    {
        services.AddSingleton<Exchange>();
        return services;
    }
}

public class Exchange
{
    private readonly RabbitMQConfig _config;
    private IConnection _connection = null!;
    private volatile bool _initialized = false;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public Exchange(IOptions<RabbitMQConfig> options)
    {
        _config = options.Value;
    }

    public async Task EnsureInitialized()
    {
        if (_initialized) return;

        try
        {
            await _semaphore.WaitAsync();
            
            if (_initialized) return;
            
            var factory = new ConnectionFactory 
            { 
                HostName = _config.HostName,
                Port = _config.Port,
                UserName = _config.UserName,
                Password = _config.Password,
            };
            
            _connection = await factory.CreateConnectionAsync();
            var channel = await _connection.CreateChannelAsync();
            
            await channel.ExchangeDeclareAsync(_config.ExchangeName, ExchangeType.Direct, 
                durable: true, autoDelete: false);
            
            _initialized = true;
            Console.WriteLine("RabbitMQ connection initialized");
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task PublishNotification(string type, EmailDetails emailDetails)
    {
        if (!_initialized)
        {
            await EnsureInitialized();
        }

        var channel = await _connection.CreateChannelAsync();
        string serializedMessage = JsonSerializer.Serialize(emailDetails);
        var body = Encoding.UTF8.GetBytes(serializedMessage);
        await channel.BasicPublishAsync(_config.ExchangeName, type, body);
        Console.WriteLine($"Published {type} notification: {emailDetails}");
    }
    
    public async Task PublishSearch(string type, string message)
    {
        if (!_initialized)
        {
            await EnsureInitialized();
        }

        var channel = await _connection.CreateChannelAsync();
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(_config.ExchangeName,type,body);
        Console.WriteLine($"Published {type} value: {message}");
    }

    ~Exchange()
    {
        _connection?.Dispose();
        _semaphore.Dispose();
    }
}