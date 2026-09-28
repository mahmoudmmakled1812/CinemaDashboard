using CinemaDashboard.Constants;
using CinemaDashboard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
public class OrdersController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Orders
        .Include(x => x.ApplicationUser).Include(x => x.Movie).ThenInclude(x => x!.Cinema)
        .Include(x => x.Seats).Include(x => x.Payment).OrderByDescending(x => x.CreatedAtUtc).ToListAsync());
}
