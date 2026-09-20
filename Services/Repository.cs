using System.Linq;
using System.Linq.Expressions;
using CinemaDashboard.Data;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Services;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _set;

    public Repository(AppDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public IQueryable<T> Get(Expression<Func<T, bool>>? filter = null)
    {
        return filter is null ? _set : _set.Where(filter);
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _set.FindAsync(id);
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _set.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _set.Update(entity);
    }

    public void Delete(T entity)
    {
        _set.Remove(entity);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    // Implemented to satisfy IRepository<T>
    public T? GetOne(Expression<Func<T, bool>>? filter = null, bool tracked = true)
    {
        IQueryable<T> query = tracked ? _set : _set.AsNoTracking();

        return filter is null ? query.FirstOrDefault() : query.FirstOrDefault(filter);
    }
}