using System.Security.Claims;
using CinemaDashboard.Constants;
using CinemaDashboard.Data;
using CinemaDashboard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Areas.Customer.Controllers;

[Area(AreaConstants.CUSTOMER_AREA)]
[Authorize(Roles = RoleConstants.CUSTOMER)]
public class OrdersController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await OrdersQuery().OrderByDescending(x => x.CreatedAtUtc).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var order = await OrdersQuery().FirstOrDefaultAsync(x => x.Id == id);
        return order is null ? NotFound() : View(order);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var order = await db.Orders.Include(x => x.Movie).Include(x => x.Seats).Include(x => x.Payment)
            .FirstOrDefaultAsync(x => x.Id == id && x.ApplicationUserId == UserId);
        if (order is null) return NotFound();
        if (order.Status == OrderStatus.Cancelled || order.Movie!.ShowDateTime.ToUniversalTime() - DateTime.UtcNow < TimeSpan.FromHours(24))
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "Tickets can only be cancelled more than 24 hours before the show.";
            return RedirectToAction(nameof(Details), new { id });
        }
        db.BookingSeats.RemoveRange(order.Seats);
        order.Status = OrderStatus.Cancelled;
        if (order.Payment is not null) order.Payment.Status = "Refunded";
        await db.SaveChangesAsync();
        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Booking cancelled and the seats are available again.";
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<Order> OrdersQuery() => db.Orders.Where(x => x.ApplicationUserId == UserId)
        .Include(x => x.Movie).ThenInclude(x => x!.Cinema).Include(x => x.Seats).Include(x => x.Payment);
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
