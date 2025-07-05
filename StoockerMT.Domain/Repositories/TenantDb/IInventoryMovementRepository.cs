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
    public interface IInventoryMovementRepository : IRepository<InventoryMovement>
    {
        Task<IReadOnlyList<InventoryMovement>> GetByProductAsync(int productId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<InventoryMovement>> GetByTypeAsync(MovementType type, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<InventoryMovement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<InventoryMovement>> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
        Task<Quantity> GetTotalQuantityByTypeAsync(int productId, MovementType type, CancellationToken cancellationToken = default);
        Task<Money> GetInventoryValueAsync(int productId, CancellationToken cancellationToken = default);
    }
}
