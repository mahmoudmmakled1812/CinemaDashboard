using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CinemaDashboard.Controllers;

public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (!User.IsInRole(CinemaDashboard.Constants.RoleConstants.ADMIN) &&
            !User.IsInRole(CinemaDashboard.Constants.RoleConstants.SUPER_ADMIN))
        {
            return RedirectToAction("Index", "Home", new { area = CinemaDashboard.Constants.AreaConstants.CUSTOMER_AREA });
        }

        return RedirectToAction("Index", "Home", new { area = CinemaDashboard.Constants.AreaConstants.ADMIN_AREA });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
