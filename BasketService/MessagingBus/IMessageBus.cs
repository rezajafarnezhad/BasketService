using BasketService.MessagingBus.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BasketService.MessagingBus;

public interface IMessageBus
{
    Task SendMessage(BaseMessage message, string queueName);
    Task ReceivedMessage(string queueName);
}


public class RabbitMqMessageBus : IMessageBus
{
    private readonly RabbitMqConfiguration _rabbitMqConfiguration;
    private readonly IRabbitMqMessageBusHelper _rabbitMqHelper;
    public RabbitMqMessageBus(IOptions<RabbitMqConfiguration> rabbitMqConfiguration, IRabbitMqMessageBusHelper rabbitMqHelper)
    {
        _rabbitMqHelper = rabbitMqHelper;
        _rabbitMqConfiguration = rabbitMqConfiguration.Value;
    }

    public async Task SendMessage(BaseMessage message, string queueName)
    {
        var connection = await _rabbitMqHelper.CheckCreateRabbitMqConnection(_rabbitMqConfiguration.HostName, _rabbitMqConfiguration.UserName, _rabbitMqConfiguration.Password);

        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: _rabbitMqConfiguration.QueueName, durable: true,
            exclusive: false, autoDelete: false, arguments: null);

        var body = _rabbitMqHelper.CreateBody(message);
        var basicProperties = new BasicProperties
        {
            Persistent = true,
        };
        await channel.BasicPublishAsync(exchange: "", routingKey: _rabbitMqConfiguration.QueueName, mandatory: false, basicProperties, body);

    }

    public async Task ReceivedMessage(string queueName)
    {
        var connection = await _rabbitMqHelper.CheckCreateRabbitMqConnection(_rabbitMqConfiguration.HostName, _rabbitMqConfiguration.UserName, _rabbitMqConfiguration.Password);

        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName, durable: true,
            exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await Task.Delay(3000);
            Console.WriteLine(message);
        };

        await channel.BasicConsumeAsync("basketQueue", true, consumer);
    }
}


public interface IRabbitMqMessageBusHelper
{
    Task<IConnection> CreateRabbitMqConnection(string hostName, string userName, string password);
    ValueTask<IConnection> CheckCreateRabbitMqConnection(string hostName, string userName, string password);
    byte[] CreateBody(BaseMessage message);
}

public class RabbitMqMessageBusHelper : IRabbitMqMessageBusHelper
{
    private static IConnection _connection;
    public async Task<IConnection> CreateRabbitMqConnection(string hostName, string userName, string password)
    {
        try
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
            };

            _connection = await connectionFactory.CreateConnectionAsync();
            return _connection;
        }
        catch (Exception e)
        {
            Console.WriteLine($"can not Create connection: {e.Message}");
            throw;
        }
    }

    public async ValueTask<IConnection> CheckCreateRabbitMqConnection(string hostName, string userName, string password)
    {
        if (_connection is not null)
            return _connection;

        return await CreateRabbitMqConnection(hostName, userName, password);
    }

    public byte[] CreateBody(BaseMessage message)
    {
        var json = JsonConvert.SerializeObject(message);
        return Encoding.UTF8.GetBytes(json);
    }
}
