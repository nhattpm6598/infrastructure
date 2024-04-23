using Infrastructure.RabbitMQ.Common.Exceptions.Base;
using System.ComponentModel;

namespace Infrastructure.RabbitMQ.Common.Exceptions
{
    public enum RabbitMQReason
    {
        [Description("")]
        CONFIG_CONNECT_FAILD,

        [Description("")]
        SETTING_OPTIONS_NOT_FOUND,

        [Description("")]
        NOT_FOUND_QUEUE,

        [Description("")]
        EXCHANGE_TYPE_INCORRECT
    }

    public class RabbitMQException : BaseExceptionReason<RabbitMQReason>
    {
        public RabbitMQException(RabbitMQReason reason) : base(reason)
        {
        }

        public RabbitMQException(RabbitMQReason reason, string message) : base(reason)
        {
        }
    }
}
