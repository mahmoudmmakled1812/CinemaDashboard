using CinemaDashboard.Constants;
using CinemaDashboard.Data;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Areas.Customer.Controllers;

[Area(AreaConstants.CUSTOMER_AREA)]
public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(new CustomerHomeVM
    {
        Movies = await db.Movies.Include(x => x.Cinema).Include(x => x.Category)
            .Where(x => x.IsAvailable && x.ShowDateTime > DateTime.Now).OrderBy(x => x.ShowDateTime).ToListAsync(),
        Cinemas = await db.Cinemas.OrderBy(x => x.Name).ToListAsync(),
        Actors = await db.Actors.OrderBy(x => x.Name).ToListAsync()
    });
}
