using System.Security.Claims;
using CinemaDashboard.Constants;
using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Areas.Customer.Controllers;

[Area(AreaConstants.CUSTOMER_AREA)]
[Authorize(Roles = RoleConstants.CUSTOMER)]
public class BookingsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> SelectSeats(int movieId)
    {
        var movie = await db.Movies.Include(x => x.Cinema).FirstOrDefaultAsync(x => x.Id == movieId && x.IsAvailable);
        if (movie is null || movie.ShowDateTime <= DateTime.Now) return NotFound();
        var reserved = await db.BookingSeats.Where(x => x.MovieId == movieId)
            .Select(x => new { x.RowNumber, x.SeatNumber }).ToListAsync();
        return View(new SeatSelectionVM { Movie = movie, ReservedSeats = reserved.Select(x => $"{x.RowNumber}-{x.SeatNumber}").ToHashSet() });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(int movieId, string? selectedSeats)
    {
        var requested = ParseSeats(selectedSeats).Distinct().ToList();
        var movie = await db.Movies.FirstOrDefaultAsync(x => x.Id == movieId && x.IsAvailable);
        if (movie is null || movie.ShowDateTime <= DateTime.Now || requested.Count == 0 ||
            requested.Any(x => x.Row < 1 || x.Row > movie.SeatRows || x.Seat < 1 || x.Seat > movie.SeatsPerRow))
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "Please choose valid available seats.";
            return RedirectToAction(nameof(SelectSeats), new { movieId });
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var taken = await db.BookingSeats.Where(x => x.MovieId == movieId)
                .AnyAsync(x => requested.Any(s => s.Row == x.RowNumber && s.Seat == x.SeatNumber));
            if (taken)
            {
                TempData[NotificationConstants.ERROR_NOTIFICATION] = "One or more selected seats were just booked. Please choose again.";
                return RedirectToAction(nameof(SelectSeats), new { movieId });
            }

            var order = new Order { ApplicationUserId = UserId, MovieId = movieId, TotalAmount = movie.Price * requested.Count };
            foreach (var seat in requested)
                order.Seats.Add(new BookingSeat { MovieId = movieId, RowNumber = seat.Row, SeatNumber = seat.Seat });
            order.Payment = new Payment { Amount = order.TotalAmount, TransactionReference = $"DEMO-{Guid.NewGuid():N}" };
            db.Orders.Add(order);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction(nameof(OrdersController.Details), "Orders", new { area = AreaConstants.CUSTOMER_AREA, id = order.Id });
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "A selected seat is no longer available. Please select again.";
            return RedirectToAction(nameof(SelectSeats), new { movieId });
        }
    }

    private static IEnumerable<(int Row, int Seat)> ParseSeats(string? values) => (values ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(value => value.Split('-')).Where(parts => parts.Length == 2 && int.TryParse(parts[0], out _) && int.TryParse(parts[1], out _))
        .Select(parts => (int.Parse(parts[0]), int.Parse(parts[1])));
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
