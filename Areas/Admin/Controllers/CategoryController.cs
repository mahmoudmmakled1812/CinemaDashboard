// Services/IRepository.cs
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace CinemaDashboard.Services;

public interface IRepository<T> where T : class
{
    IQueryable<T> Get(Expression<Func<T, bool>>? filter = null);

    Task<T?> GetByIdAsync(int id);

    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);

    Task CommitAsync(CancellationToken cancellationToken = default);

    // Use a strongly-typed expression returning T? so lambdas like `e => e.Id == id` bind to T
    T? GetOne(Expression<Func<T, bool>>? filter = null, bool tracked = true);

    // Remove the incorrect Delete(string) overload (already have Delete(T))
}

[Area("Admin")]
public class CategoryController : Controller
{
    // Add actions / constructor injections you actually need here.
    public IActionResult Index()
    {
        return View();
    }
}