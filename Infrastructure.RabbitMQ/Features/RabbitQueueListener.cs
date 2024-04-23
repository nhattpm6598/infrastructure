using Infrastructure.RabbitMQ.Common.Extensions;
using Infrastructure.RabbitMQ.Features.Core;
using Infrastructure.RabbitMQ.Features.Interface;
using Infrastructure.RabbitMQ.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Infrastructure.RabbitMQ.Features
{
    public class RabbitQueueListener<T> : BackgroundService where T : class
    {
        private readonly ILogger<RabbitQueueListener<T>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitManager _manager;
        private readonly QueueManagerSetting _queueManagerSetting;
        private readonly string _queueName;

        public RabbitQueueListener(ILogger<RabbitQueueListener<T>> logger,
            IServiceProvider serviceProvider,
            QueueManagerSetting queueManagerSetting,
            IRabbitManager manager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _queueManagerSetting = queueManagerSetting;
            _manager = manager;
            _queueName = _queueManagerSetting.Items.First(q => q.Type == typeof(T)).Name;
        }

        /// <summary>
        /// ExecuteAsync
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_manager.Channel);
            consumer.Received += async (_, message) =>
            {
                try
                {
                    _logger.LogDebug("Messaged received: {Request}", Encoding.UTF8.GetString(message.Body.Span));

                    using var scope = _serviceProvider.CreateScope();

                    var receiver = scope.ServiceProvider.GetRequiredService<IRabbitMessageReceiver<T>>();
                    var response = JsonSerializer.Deserialize<T>(message.Body.Span, JsonOptions.Default);
                    await receiver.ReceiveAsync(response!, stoppingToken);

                    _manager.MarkAsComplete(message);

                    _logger.LogDebug("Message processed");
                }
                catch (Exception ex)
                {
                    _manager.MarkAsRejected(message);
                    _logger.LogError(ex, "Unexpected error while processing message");
                }

                stoppingToken.ThrowIfCancellationRequested();
            };

            _manager.Channel.BasicConsume(_queueName, autoAck: false, consumer);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
            _manager.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// StartAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("RabbitMQ Listener for {QueueName} started", _queueName);

            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// StopAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("RabbitMQ Listener for {QueueName} stopped", _queueName);

            return base.StopAsync(cancellationToken);
        }
    }
}
