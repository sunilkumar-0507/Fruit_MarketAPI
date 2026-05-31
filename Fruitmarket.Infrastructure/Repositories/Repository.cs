using System.Linq.Expressions;
using Fruitmarket.Application.Abstractions;
using Fruitmarket.Domain.Common;
using Fruitmarket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fruitmarket.Infrastructure.Repositories;

public sealed class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = context.Set<T>();
    public IQueryable<T> Query() => _set.AsQueryable();
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => _set.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => _set.FirstOrDefaultAsync(predicate, cancellationToken);
    public Task AddAsync(T entity, CancellationToken cancellationToken = default) => _set.AddAsync(entity, cancellationToken).AsTask();
    public void Update(T entity) => _set.Update(entity);
    public void Remove(T entity) => _set.Remove(entity);
}
