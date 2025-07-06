using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoockerMT.Persistence.Repositories.TenantDb
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private readonly TenantDbContext _context;

        public OrderRepository(TenantDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order?> GetByNumberAsync(OrderNumber orderNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderNumber.Value == orderNumber.Value, cancellationToken);
        }

        public async Task<Order?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Confirmed)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetOverdueOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o =>
                    o.RequiredDate.HasValue &&
                    o.RequiredDate.Value < DateTime.UtcNow &&
                    o.Status != OrderStatus.Delivered &&
                    o.Status != OrderStatus.Cancelled)
                .ToListAsync(cancellationToken);
        }

        public async Task<string> GenerateNextOrderNumberAsync(CancellationToken cancellationToken = default)
        {
            var year = DateTime.UtcNow.Year;
            var yearPrefix = $"ORD-{year}-";

            var lastOrder = await _context.Orders
                .Where(o => o.OrderNumber.Value.StartsWith(yearPrefix))
                .OrderByDescending(o => o.OrderNumber.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastOrder == null)
            {
                return $"{yearPrefix}000001";
            }

            var lastNumber = lastOrder.OrderNumber.Value.Substring(yearPrefix.Length);
            if (int.TryParse(lastNumber, out var number))
            {
                return $"{yearPrefix}{(number + 1):D6}";
            }

            return $"{yearPrefix}000001";
        }

        public async Task<Money> GetTotalSalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var orders = await _context.Orders
                .Where(o =>
                    o.OrderDate >= startDate &&
                    o.OrderDate <= endDate &&
                    o.Status == OrderStatus.Delivered)
                .ToListAsync(cancellationToken);

            if (!orders.Any())
                return Money.Zero("USD");

            var currency = orders.First().Total.Currency;
            var totalAmount = orders.Sum(o => o.Total.Amount);

            return new Money(totalAmount, currency);
        }
    }
}
