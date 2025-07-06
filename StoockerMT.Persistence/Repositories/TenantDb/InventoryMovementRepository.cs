using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;

namespace StoockerMT.Persistence.Repositories.TenantDb
{
    public class InventoryMovementRepository : RepositoryBase<InventoryMovement>, IInventoryMovementRepository
    {
        private readonly TenantDbContext _context;

        public InventoryMovementRepository(TenantDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<InventoryMovement>> GetByProductAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _context.InventoryMovements
                .Where(m => m.ProductId == productId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<InventoryMovement>> GetByTypeAsync(MovementType type, CancellationToken cancellationToken = default)
        {
            return await _context.InventoryMovements
                .Where(m => m.Type == type)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<InventoryMovement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.InventoryMovements
                .Where(m => m.MovementDate >= startDate && m.MovementDate <= endDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<InventoryMovement>> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
        {
            return await _context.InventoryMovements
                .Where(m => m.Reference == reference)
                .ToListAsync(cancellationToken);
        }

        public async Task<Quantity> GetTotalQuantityByTypeAsync(int productId, MovementType type, CancellationToken cancellationToken = default)
        {
            var inventories = await _context.InventoryMovements
                .Where(m => m.ProductId == productId && m.Type == type)
                .ToListAsync(cancellationToken);

            return new Quantity(inventories.Sum(m => m.Quantity.Value), inventories.FirstOrDefault()?.Quantity.Unit ?? "");
        }

        public async Task<Money> GetInventoryValueAsync(int productId, CancellationToken cancellationToken = default)
        {
            var inventories = await _context.InventoryMovements
                .Where(m => m.ProductId == productId)
                .ToListAsync(cancellationToken);

            if (!inventories.Any())
                return Money.Zero("USD");

            var currency = inventories.First().UnitCost.Currency;
            var totalValue = inventories.Sum(m => m.GetTotalValue().Amount);

            return new Money(totalValue, currency);
        }
    }
}
