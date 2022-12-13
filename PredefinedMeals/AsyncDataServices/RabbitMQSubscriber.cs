using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PredefinedMeals.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PredefinedMeals.AsyncDataServices
{
    public class RabbitMQSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public RabbitMQSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            Console.WriteLine($"--> Hostname {_configuration["RabbitMQ:HostName"]} {_configuration["RabbitMQ:Port"]}");
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:HostName"], Port = int.Parse(_configuration["RabbitMQ:Port"]) };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel(); 
                _channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:ProductRemove:Exchange"], type: ExchangeType.Fanout);
                _channel.QueueDeclare(_configuration["RabbitMQ:ProductRemove:QueueName"], false, false, false, null);
                _channel.QueueBind(queue: _configuration["RabbitMQ:ProductRemove:QueueName"], _configuration["RabbitMQ:ProductRemove:Exchange"], routingKey:"");

                Console.WriteLine("--> Listening on the RabbitMQ Message Bus...");
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Could not connect to RabbitMQ Message Bus: {ex.Message}");
            }

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            if(_channel is not null)
            {
                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (ModuleHandle, ea) =>
                {
                    Console.WriteLine("--> Event received!");

                    var body = ea.Body;

                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine($"--> received: {message}");

                    _eventProcessor.ProcessEventAsync(message);
                };

                _channel.BasicConsume(queue: _configuration["RabbitMQ:ProductRemove:QueueName"], autoAck: true, consumer: consumer);
            }
            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection Shutdown");
        }

        public override void Dispose()
        {
            if(_channel is not null && _channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}