using Infrastructure.RabbitMQ.Common.Hepler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.RabbitMQ.Common.Exceptions.Base
{
#nullable disable

    public class BaseExceptionReason<TReason> : BaseException
        where TReason : Enum
    {
        public BaseExceptionReason(TReason reason)
        {
            this.Reason = reason;
            this.ErrorCode = reason.ToString();
            this.ErrorMessage = reason.DescriptionAttr();
        }

        public BaseExceptionReason(TReason reason, string message)
        {
            this.Reason = reason;
            this.ErrorCode = reason.ToString();
            this.ErrorMessage = message;
        }

        public TReason Reason { get; set; }

    }

    public class BaseException : Exception
    {

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
