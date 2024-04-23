using Infrastructure.RabbitMQ.Common.Exceptions;
using Infrastructure.RabbitMQ.Common.Extensions;
using Infrastructure.RabbitMQ.Features.Core;
using Infrastructure.RabbitMQ.Features.Interface;
using Infrastructure.RabbitMQ.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.RabbitMQ.Features
{
    public class RabbitMessageSender : RabbitManager, IRabbitMessageSender
    {
        public RabbitMessageSender(IConnectionFactory connectionFactory,
            ExchangeManagerSetting exchangeSetting,
            QueueManagerSetting queueSetting) : base(connectionFactory, exchangeSetting, queueSetting)
        {
        }

        /// <summary>
        /// Sender
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        /// <exception cref="RabbitMQException"></exception>
        public Task PublishAsync<T>(T message, int priority = 1) where T : class
        {
            var sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<object>(message, JsonOptions.Default));

            QueueSetting? setting = _queueSetting.Items.Where(_ => _.Type == typeof(T)).FirstOrDefault();

            if (setting == null) throw new RabbitMQException(RabbitMQReason.NOT_FOUND_QUEUE);

            return PublishAsync(sendBytes.AsMemory(), setting.Exchange, setting.Name, priority);
        }
    }
}
