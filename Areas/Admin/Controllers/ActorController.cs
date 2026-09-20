// Areas/Admin/Controllers/CategoryController.cs
using CinemaDashboard.Constants;
using CinemaDashboard.Models;
using CinemaDashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaDashboard.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
public class CategoryController : Controller
{
    private readonly IRepository<Category> _repository;

    public CategoryController(IRepository<Category> repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        // Minimal placeholder; replace with real implementation as needed
        return View();
    }
}