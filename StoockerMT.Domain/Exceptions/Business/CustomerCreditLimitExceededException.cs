using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Business
{
    public class CustomerCreditLimitExceededException : DomainException
    {
        public int CustomerId { get; }
        public Money OrderAmount { get; }
        public Money CreditLimit { get; }
        public Money CurrentBalance { get; }

        public CustomerCreditLimitExceededException(int customerId, Money orderAmount,
            Money creditLimit, Money currentBalance)
            : base("CREDIT_LIMIT_EXCEEDED",
                $"Customer '{customerId}' credit limit exceeded. Order: {orderAmount}, " +
                $"Limit: {creditLimit}, Current Balance: {currentBalance}")
        {
            CustomerId = customerId;
            OrderAmount = orderAmount;
            CreditLimit = creditLimit;
            CurrentBalance = currentBalance;
        }
    }
}
