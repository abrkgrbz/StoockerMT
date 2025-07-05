using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Business
{
    public class InvalidOrderStateException : DomainException
    {
        public int OrderId { get; }
        public OrderStatus CurrentStatus { get; }
        public string AttemptedAction { get; }

        public InvalidOrderStateException(int orderId, OrderStatus currentStatus, string attemptedAction)
            : base("INVALID_ORDER_STATE",
                $"Cannot {attemptedAction} order '{orderId}' in status '{currentStatus}'.")
        {
            OrderId = orderId;
            CurrentStatus = currentStatus;
            AttemptedAction = attemptedAction;
        }
    }
}
