using CinemaDashboard.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaDashboard.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult NotFoundPage()
    {
        return View();
    }
}