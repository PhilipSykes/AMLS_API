using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Common.Configuration;
using Common.Constants;
using Services.NotificationService.Handlers;

namespace Services.NotificationService;
public class NotificationMessageReceiver : BackgroundService
{
    private readonly RabbitMQConfig _config;
    private static readonly string[] NotificationTypes =[
        MessageTypes.Notifications.Login,
        MessageTypes.Notifications.Logout,
        MessageTypes.Notifications.PasswordReset,
        MessageTypes.Notifications.ProfileUpdate
    ];

    
    public NotificationMessageReceiver(IOptions<RabbitMQConfig> options)
    {
        _config = options.Value;
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
        
        foreach(var type in NotificationTypes)
        {
            await channel.QueueBindAsync(queueName, _config.ExchangeName, type);
            Console.WriteLine($"Bound to {type} notifications");
        }

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var notificationType = ea.RoutingKey;

            await NotificationHandler.HandleNotification(notificationType, message);
        };

        await channel.BasicConsumeAsync(queueName, true, consumer);

        // Keep the service running until cancellation is requested
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
        }
    }
}