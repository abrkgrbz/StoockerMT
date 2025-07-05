using StoockerMT.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> FindAsync(Specification<T> specification, CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync(Specification<T> specification, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Specification<T> specification, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Specification<T> specification, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
