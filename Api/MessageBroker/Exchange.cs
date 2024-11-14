using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using Common.Configuration;

namespace Api.MessageBroker;
public class Exchange(IOptions<RabbitMQConfig> options)
    {
        private readonly RabbitMQConfig _config = options.Value;
        private IConnection _connection = null!;
        public async Task InitializeConnection()
        {
             var factory = new ConnectionFactory 
            { 
                HostName = _config.HostName,
                Port = _config.Port,
                UserName = _config.UserName,
                Password = _config.Password
            };
            
            _connection = await factory.CreateConnectionAsync();
            var channel = await _connection.CreateChannelAsync();
    
            await channel.ExchangeDeclareAsync(_config.ExchangeName, ExchangeType.Direct);
            Console.WriteLine("RabbitMQ connection initialized");
        }
    
        public async Task PublishWeatherRequest()
        {
            var channel = await _connection.CreateChannelAsync();
            var body = Encoding.UTF8.GetBytes("weather.request");
            await channel.BasicPublishAsync(_config.ExchangeName, "weather", body);
            Console.WriteLine("Published weather request");
        }
        
        public async Task PublishNotification(string type, string message)
            {
                var channel = await _connection.CreateChannelAsync();
                var body = Encoding.UTF8.GetBytes(message);
                await channel.BasicPublishAsync(_config.ExchangeName, type, body);
                Console.WriteLine($"Published {type} notification: {message}");
            }
    }