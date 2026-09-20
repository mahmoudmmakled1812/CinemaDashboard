using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Controllers;

public class CinemasController : Controller
{
    private readonly AppDbContext db;
    private readonly IImageService images;

    public CinemasController(AppDbContext db, IImageService images)
    {
        this.db = db;
        this.images = images;
    }

    public async Task<IActionResult> Index()
    {
        var cinemas = await db.Cinemas
            .OrderBy(x => x.Name)
            .ToListAsync();

        return View(cinemas);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cinema cinema, IFormFile? image)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", await db.Cinemas.ToListAsync());
        }

        cinema.ImageUrl = await images.SaveAsync(image, "cinemas");
        db.Cinemas.Add(cinema);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var cinema = await db.Cinemas.FindAsync(id);

        if (cinema is null)
            return NotFound();

        if (await db.Movies.AnyAsync(x => x.CinemaId == id))
        {
            TempData["Error"] = "Delete or reassign the movies in this cinema first.";
            return RedirectToAction(nameof(Index));
        }

        images.Delete(cinema.ImageUrl);
        db.Cinemas.Remove(cinema);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}