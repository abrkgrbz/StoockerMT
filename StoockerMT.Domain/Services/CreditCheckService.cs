using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Services
{
    public interface ICreditCheckService
    {
        bool CanPlaceOrder(Customer customer, Money orderAmount);
        Money GetAvailableCredit(Customer customer, IEnumerable<Order> openOrders);
    }

    public class CreditCheckService : ICreditCheckService
    {
        public bool CanPlaceOrder(Customer customer, Money orderAmount)
        {
            if (customer.Status != CustomerStatus.Active)
                return false;

            // Business customers get special treatment
            if (customer.Type == CustomerType.Business)
                return true;

            return orderAmount.Amount <= customer.CreditLimit.Amount;
        }

        public Money GetAvailableCredit(Customer customer, IEnumerable<Order> openOrders)
        {
            var openOrdersTotal = openOrders
                .Where(o => o.Status != OrderStatus.Delivered &&
                            o.Status != OrderStatus.Cancelled)
                .Aggregate(
                    Money.Zero(customer.CreditLimit.Currency),
                    (sum, order) => sum.Add(order.Total)
                );

            return customer.CreditLimit.Subtract(openOrdersTotal);
        }
    }
}
