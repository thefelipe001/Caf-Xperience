using CafeXperienceApp.Models;
using CafeXperienceApp.Repositorio;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeXperienceApp.Controllers
{
    public class ArticuloComprasController : Controller
    {
        private readonly ApplicationDbContext _context;


        public ArticuloComprasController(ApplicationDbContext context)
        {
           _context = context;

        }

        // Carrito de compras simulado
        private static List<Articulo> Carrito = new List<Articulo>();

        // Acción para mostrar los artículos disponibles
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
            ViewData["saldo"] = User.Claims.FirstOrDefault(c => c.Type == "LimiteCredito")?.Value;
            ViewData["Rol"] = User.Claims.FirstOrDefault(c => c.Type == "Rol")?.Value;
            var articulosDisponibles = _context.Articulos.Where(a => a.Estado == "A" && a.Existencia > 0).ToList();  // Filtrar solo los artículos disponibles y activos
            return View(articulosDisponibles);  // Pasar la lista de artículos a la vista
        }

        [HttpPost]
        public JsonResult AñadirAlCarrito(int id)
        {
            try
            {
                // Lógica para añadir el artículo al carrito
                // Por ejemplo: guardar en la sesión o base de datos

                return Json(new { success = true, message = "Artículo añadido al carrito" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al añadir el artículo: " + ex.Message });
            }
        }







    }
}
