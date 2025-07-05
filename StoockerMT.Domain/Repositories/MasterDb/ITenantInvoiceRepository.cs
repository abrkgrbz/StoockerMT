using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.MasterDb
{
    public interface ITenantInvoiceRepository : IRepository<TenantInvoice>
    {
        Task<TenantInvoice?> GetByNumberAsync(InvoiceNumber invoiceNumber, CancellationToken cancellationToken = default);
        Task<TenantInvoice?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantInvoice>> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantInvoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantInvoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantInvoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<Money> GetTotalPendingAmountAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<string> GenerateNextInvoiceNumberAsync(CancellationToken cancellationToken = default);
    }
}
