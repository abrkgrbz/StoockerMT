using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Domain.Repositories.UnitOfWork;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.TenantDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Repositories.UnitOfWork
{
    public class TenantDbUnitOfWork : Common.UnitOfWork, ITenantDbUnitOfWork
    {
        private readonly TenantDbContext _context;

        // Lazy initialization of repositories
        private ICustomerRepository? _customers;
        private IProductRepository? _products;
        private IOrderRepository? _orders;
        private IEmployeeRepository? _employees;
        private IAccountRepository? _accounts;
        private IJournalEntryRepository? _journalEntries;
        private IInventoryMovementRepository? _inventoryMovements;
        private IDepartmentRepository? _departments;
        private IPositionRepository? _positions;
        private IProductCategoryRepository? _productCategories;

        public TenantDbUnitOfWork(TenantDbContext context) : base(context)
        {
            _context = context;
        }

        public ICustomerRepository Customers =>
            _customers ??= new CustomerRepository(_context);

        public IProductRepository Products =>
            _products ??= new ProductRepository(_context);

        public IOrderRepository Orders =>
            _orders ??= new OrderRepository(_context);

        public IEmployeeRepository Employees =>
            _employees ??= new EmployeeRepository(_context); 

        public IAccountRepository Accounts =>
            _accounts ??= new AccountRepository(_context);

        public IJournalEntryRepository JournalEntries =>
            _journalEntries ??= new JournalEntryRepository(_context);

        public IInventoryMovementRepository InventoryMovements =>
            _inventoryMovements ??= new InventoryMovementRepository(_context);

        public IDepartmentRepository Departments =>
            _departments ??= new DepartmentRepository(_context);

        public IPositionRepository Positions =>
            _positions ??= new PositionRepository(_context);

        public IProductCategoryRepository ProductCategories =>
            _productCategories ??= new ProductCategoryRepository(_context);
    }
}
