using CafeXperienceApp.Models;
using CafeXperienceApp.Repositorio;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeXperienceApp.Controllers
{
    public class ArticuloComprasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CarritoService _carritoService;


        public ArticuloComprasController(ApplicationDbContext context, CarritoService carritoService)
        {
           _context = context;
           _carritoService = carritoService;

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

       

    }
}
