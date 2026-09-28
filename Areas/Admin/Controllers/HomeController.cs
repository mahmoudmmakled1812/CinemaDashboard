using CinemaDashboard.Constants;
using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace CinemaDashboard.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var recentOrders = await db.Orders.Include(x => x.Movie).Include(x => x.ApplicationUser).Include(x => x.Seats)
            .OrderByDescending(x => x.CreatedAtUtc).Take(6).ToListAsync();
        return View(new AdminDashboardVM
        {
            Movies = await db.Movies.CountAsync(),
            Cinemas = await db.Cinemas.CountAsync(),
            Customers = await db.Users.CountAsync(),
            ConfirmedOrders = await db.Orders.CountAsync(x => x.Status == OrderStatus.Confirmed),
            Revenue = await db.Orders.Where(x => x.Status == OrderStatus.Confirmed).SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
            RecentOrders = recentOrders
        });
    }

    public IActionResult NotFoundPage()
    {
        return View();
    }
}
