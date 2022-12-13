using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PredefinedMeals.Dtos.RabbitMQ.Request;
using RabbitMQ.Client;

namespace PredefinedMeals.AsyncDataServices.RabbitMQ
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() 
            {
                 HostName = _configuration["RabbitMQ:HostName"],
                 Port = int.Parse(_configuration["RabbitMQ:Port"])
            };

            Console.WriteLine($"--> Hostname {_configuration["RabbitMQ:HostName"]} {_configuration["RabbitMQ:Port"]}");

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:LogPublish:Exchange"], type: ExchangeType.Direct);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("Connected to the message bus");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Could not connect to RabbitMQ Message Bus: {ex.Message}");
            }
        }

        public void PublishNewLog(LogRequestDto log)
        {
            var message = JsonSerializer.Serialize(log);

            if(_connection is not null && _connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection is Open, sending message...");

                try{
                    SendMessage(message);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Could not send asynchronously: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection is Closed, not sending...");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: _configuration["RabbitMQ:LogPublish:Exchange"],
                        routingKey: _configuration["RabbitMQ:LogPublish:RoutingKey"],
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