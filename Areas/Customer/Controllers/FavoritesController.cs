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
public class FavoritesController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.FavoriteItems
        .Where(x => x.ApplicationUserId == UserId)
        .Include(x => x.Movie).ThenInclude(x => x!.Cinema)
        .OrderByDescending(x => x.CreatedAtUtc).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int movieId, string? returnUrl = null)
    {
        var favorite = await db.FavoriteItems.FirstOrDefaultAsync(x => x.ApplicationUserId == UserId && x.MovieId == movieId);
        if (favorite is null)
            db.FavoriteItems.Add(new FavoriteItem { ApplicationUserId = UserId, MovieId = movieId });
        else
            db.FavoriteItems.Remove(favorite);
        await db.SaveChangesAsync();
        return LocalRedirect(!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action(nameof(Index))!);
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
