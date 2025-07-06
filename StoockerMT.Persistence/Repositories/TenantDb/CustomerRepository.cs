using Microsoft.EntityFrameworkCore;
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

namespace StoockerMT.Persistence.Repositories.TenantDb
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        private readonly TenantDbContext _context;

        public CustomerRepository(TenantDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerCode == code, cancellationToken);
        }

        public async Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email != null && c.Email.Value == email.Value, cancellationToken);
        }

        public async Task<Customer?> GetWithOrdersAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.Items)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Customer?> GetWithContactsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Customer>> GetByTypeAsync(CustomerType type, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .Where(c => c.Type == type)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Customer>> GetByStatusAsync(CustomerStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .Where(c => c.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .Where(c => EF.Functions.Like(c.CustomerName, $"%{searchTerm}%"))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AnyAsync(c => c.CustomerCode == code, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AnyAsync(c => c.Email != null && c.Email.Value == email.Value, cancellationToken);
        }

        public async Task<Money> GetTotalOutstandingBalanceAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var openOrders = await _context.Orders
                .Where(o =>
                    o.CustomerId == customerId &&
                    o.Status != OrderStatus.Cancelled &&
                    o.Status != OrderStatus.Delivered)
                .ToListAsync(cancellationToken);

            if (!openOrders.Any())
                return Money.Zero("USD");

            var currency = openOrders.First().Total.Currency;
            var totalAmount = openOrders.Sum(o => o.Total.Amount);

            return new Money(totalAmount, currency);
        }
    }
}
