using StoockerMT.Domain.Repositories.TenantDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.UnitOfWork
{
    public interface ITenantDbUnitOfWork : IUnitOfWork
    {
        ICustomerRepository Customers { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IEmployeeRepository Employees { get; }
        IAccountRepository Accounts { get; }
        IJournalEntryRepository JournalEntries { get; }
        IInventoryMovementRepository InventoryMovements { get; }
        IDepartmentRepository Departments { get; }
        IPositionRepository Positions { get; }
        IProductCategoryRepository ProductCategories { get; }
    }
}
