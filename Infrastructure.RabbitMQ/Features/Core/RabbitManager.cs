using Infrastructure.RabbitMQ.Common.Exceptions;
using Infrastructure.RabbitMQ.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.RabbitMQ.Features.Core
{
    public class RabbitManager : IRabbitManager
    {
        protected readonly IConnection _connection;
        protected readonly IModel _channel;
        protected readonly ExchangeManagerSetting _exchangeSetting;
        protected readonly QueueManagerSetting _queueSetting;

        public IModel Channel => _channel;

        public RabbitManager(IConnectionFactory connectionFactory,
            ExchangeManagerSetting exchangeSetting,
            QueueManagerSetting queueSetting)
        {
            if (connectionFactory is null) throw new RabbitMQException(RabbitMQReason.CONFIG_CONNECT_FAILD);

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeSetting = exchangeSetting;
            _queueSetting = queueSetting;

            Init();

        }

        #region init

        /// <summary>
        /// Declare
        /// </summary>
        private void Init()
        {
            // declare exchange
            foreach (var exchange in _exchangeSetting.Items)
            {
                _channel.ExchangeDeclare(exchange.Name, exchange.Type, durable: true, autoDelete: false);
            }

            // declare queue
            foreach (var queue in _queueSetting.Items)
            {
                _channel.QueueDeclare(queue: queue.Name, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(queue.Name, queue.Exchange, queue.Name, null);
            }
        }

        #endregion

        #region publish / sender

        /// <summary>
        /// sender
        /// </summary>
        /// <param name="body"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        protected Task PublishAsync(ReadOnlyMemory<byte> body, string exchange, string routingKey, int priority = 1)
        {
            var properties = _channel.CreateBasicProperties();

            properties.Persistent = true;

            properties.Priority = Convert.ToByte(priority);

            _channel.BasicPublish(exchange, routingKey, properties, body);

            return Task.CompletedTask;
        }

        #endregion

        #region update message consumer

        /// <summary>
        /// MarkAsComplete
        /// </summary>
        /// <param name="message"></param>
        public void MarkAsComplete(BasicDeliverEventArgs message) => _channel.BasicAck(message.DeliveryTag, false);

        /// <summary>
        /// MarkAsRejected
        /// </summary>
        /// <param name="message"></param>
        public void MarkAsRejected(BasicDeliverEventArgs message) => _channel.BasicReject(message.DeliveryTag, false);

        #endregion

        #region dispose

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_channel.IsOpen) _channel.Close();

                if (_connection.IsOpen) _connection.Close();
            }
            catch
            {
                throw;
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
