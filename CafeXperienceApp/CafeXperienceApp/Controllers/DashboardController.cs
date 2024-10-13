using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CafeXperienceApp.Models;

namespace CafeXperienceApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {

            ClaimsPrincipal claimUser = HttpContext.User;
            string userName = "Usuario no autenticado";

            if (claimUser?.Identity?.IsAuthenticated == true)
            {
                userName = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value)
                    .SingleOrDefault() ?? "Claim no disponible";
            }

            ViewData["userName"] = userName;

            return View();

        }
    }
}
