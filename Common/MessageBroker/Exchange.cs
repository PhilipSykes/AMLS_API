using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using static Common.Models.Shared;
using Microsoft.Extensions.DependencyInjection;



namespace Common.MessageBroker
{
    public static IServiceCollection AddMessageBroker(this IServiceCollection services)
    {
        services.AddSingleton<Exchange>();
        return services;
    }
    

    public class Exchange
    {
        private readonly RabbitMQConfig _config;
        private IConnection _connection = null!;
        private bool _initialized = false;
        private readonly object _lock = new();

        public Exchange(IOptions<RabbitMQConfig> options)
        {
            _config = options.Value;
        }

        public async Task EnsureInitialized()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                var factory = new ConnectionFactory
                {
                    HostName = _config.HostName,
                    Port = _config.Port,
                    UserName = _config.UserName,
                    Password = _config.Password,
                };

                _connection = factory.CreateConnection();
                using var channel = _connection.CreateChannel();
                channel.ExchangeDeclare(_config.ExchangeName, ExchangeType.Direct);

                _initialized = true;
                Console.WriteLine("RabbitMQ connection initialized");
            }
        }

        public async Task PublishNotification(string type, EmailDetails emailDetails)
        {
            var channel = await _connection.CreateChannelAsync();
            string serializedMessage = JsonSerializer.Serialize(emailDetails);
            var body = Encoding.UTF8.GetBytes(serializedMessage);
            await channel.BasicPublishAsync(_config.ExchangeName, type, body);
            Console.WriteLine($"Published {type} notification: {emailDetails}");
        }

        public async Task PublishSearch(string type, string message)
        {
            var channel = await _connection.CreateChannelAsync();
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(_config.ExchangeName, type, body);
            Console.WriteLine($"Published {type} value: {message}");
        }
    }
}