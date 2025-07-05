using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.TenantDb
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetByNumberAsync(OrderNumber orderNumber, CancellationToken cancellationToken = default);
        Task<Order?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetOverdueOrdersAsync(CancellationToken cancellationToken = default);
        Task<string> GenerateNextOrderNumberAsync(CancellationToken cancellationToken = default);
        Task<Money> GetTotalSalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }

}
