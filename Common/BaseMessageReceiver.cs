using System.Text;
using Common.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;

namespace Common;

public abstract class BaseMessageReceiver : BackgroundService
{
    private readonly RabbitMQConfig _config;
    private readonly string[] _messageTypes;

    protected BaseMessageReceiver(IOptions<RabbitMQConfig> options, string[] messageTypes)
    {
        _config = options.Value;
        _messageTypes = messageTypes;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory 
        { 
            HostName = _config.HostName,
            Port = _config.Port,
            UserName = _config.UserName,
            Password = _config.Password
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(_config.ExchangeName, ExchangeType.Direct);
        var queueName = (await channel.QueueDeclareAsync()).QueueName;
        
        foreach(var type in _messageTypes)
        {
            await channel.QueueBindAsync(queueName, _config.ExchangeName, type);
            Console.WriteLine($"Bound to {type}");
        }

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var messageType = ea.RoutingKey;

            await HandleMessage(messageType, message);
        };

        await channel.BasicConsumeAsync(queueName, true, consumer);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Service shutting down...");
        }
    }

    // Abstract method that each service must implement
    protected abstract Task HandleMessage(string messageType, string message);
}