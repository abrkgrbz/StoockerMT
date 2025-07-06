using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Repositories.MasterDb;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoockerMT.Persistence.Repositories.MasterDb
{
    public class TenantInvoiceRepository : RepositoryBase<TenantInvoice>, ITenantInvoiceRepository
    {
        private readonly MasterDbContext _context;

        public TenantInvoiceRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TenantInvoice?> GetByNumberAsync(InvoiceNumber invoiceNumber, CancellationToken cancellationToken = default)
        {
            return await _context.TenantInvoices
                .FirstOrDefaultAsync(i => i.InvoiceNumber.Value == invoiceNumber.Value, cancellationToken);
        }

        public async Task<TenantInvoice?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TenantInvoices
                .Include(i => i.Items)
                    .ThenInclude(item => item.Module)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<TenantInvoice>> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantInvoices
                .Where(i => i.TenantId == tenantId)
                .OrderByDescending(i => i.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantInvoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.TenantInvoices
                .Where(i => i.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantInvoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TenantInvoices
                .Where(i =>
                    i.Status != InvoiceStatus.Paid &&
                    i.Status != InvoiceStatus.Cancelled &&
                    i.DueDate < DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantInvoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.TenantInvoices
                .Where(i => i.DueDate >= startDate && i.DueDate <= endDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Money> GetTotalPendingAmountAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            var pendingInvoices = await _context.TenantInvoices
                .Where(i =>
                    i.TenantId == tenantId &&
                    i.Status != InvoiceStatus.Paid &&
                    i.Status != InvoiceStatus.Cancelled)
                .ToListAsync(cancellationToken);

            if (!pendingInvoices.Any())
                return Money.Zero("USD");

            var currency = pendingInvoices.First().Total.Currency;
            var totalAmount = pendingInvoices.Sum(i => i.Total.Amount);

            return new Money(totalAmount, currency);
        }

        public async Task<string> GenerateNextInvoiceNumberAsync(CancellationToken cancellationToken = default)
        {
            var year = DateTime.UtcNow.Year;
            var yearPrefix = $"INV-{year}-";

            var lastInvoice = await _context.TenantInvoices
                .Where(i => i.InvoiceNumber.Value.StartsWith(yearPrefix))
                .OrderByDescending(i => i.InvoiceNumber.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastInvoice == null)
            {
                return $"{yearPrefix}000001";
            }

            var lastNumber = lastInvoice.InvoiceNumber.Value.Substring(yearPrefix.Length);
            if (int.TryParse(lastNumber, out var number))
            {
                return $"{yearPrefix}{(number + 1):D6}";
            }

            return $"{yearPrefix}000001";
        }
    }
}
