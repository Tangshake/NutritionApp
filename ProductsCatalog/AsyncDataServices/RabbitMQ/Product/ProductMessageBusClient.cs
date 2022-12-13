using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ProductsCatalog.Dtos.RabbitMQ;
using RabbitMQ.Client;

namespace ProductsCatalog.AsyncDataService.RabbitMQ.Product
{
    public class ProductMessageBusClient : IProductMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public ProductMessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() 
            {
                 HostName = _configuration["RabbitMQ:HostName"],
                 Port = int.Parse(_configuration["RabbitMQ:Port"])
            };

            Console.WriteLine($"--> Hostname {_configuration["RabbitMQ:HostName"]} {_configuration["RabbitMQPort:Port"]}");

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:ProductRemove:Exchange"], type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("Connected to the message bus");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Could not connect to RabbitMQ Message Bus: {ex.Message}");
            }
        }

        public void PublishProductRemoved(int id)
        {
            var message = JsonSerializer.Serialize(id);

            if(_connection is not null && _connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection is Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection is Closed, not sending...");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var exchangeName = _configuration["RabbitMQ:ProductRemove:Exchange"];

            _channel.BasicPublish(exchange: exchangeName,
                        routingKey: "",
                        basicProperties: null,
                        body: body);

            Console.WriteLine($"We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message Bus Disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbbitMQ Connection Shutdown");
        }


    }
}