using CinemaDashboard.Constants;
using CinemaDashboard.Data;
using CinemaDashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaDashboard.Areas.Admin.Controllers;

// Exposes the existing movie-management actions at /Admin/Movies as well as the legacy route.
[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.SUPER_ADMIN}")]
public class MoviesController : CinemaDashboard.Controllers.MoviesController
{
    public MoviesController(AppDbContext db, IImageService images) : base(db, images)
    {
    }
}
