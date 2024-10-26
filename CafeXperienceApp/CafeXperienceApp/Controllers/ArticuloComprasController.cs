using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Http;
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

        // Acción para añadir un artículo al carrito
        public IActionResult AñadirAlCarrito(int id)
        {
            // Buscar el artículo por su ID
            var articulo = _context.Articulos.FirstOrDefault(a => a.IdArticulo == id);

            if (articulo != null && articulo.Existencia > 0 && articulo.Estado == "A")
            {
                // Añadir el artículo al carrito
                Carrito.Add(articulo);
                TempData["Message"] = "Artículo añadido al carrito.";
            }
            else
            {
                TempData["ErrorMessage"] = "Artículo no disponible o agotado.";
            }

            // Redirigir de vuelta a la lista de artículos
            return RedirectToAction("Index");
        }

        // Acción para mostrar el carrito
        public IActionResult VerCarrito()
        {
            return View(Carrito);  // Pasar la lista de artículos del carrito a la vista
        }

        // Acción para realizar la compra (simulación)
        public IActionResult ProcesarCompra()
        {
            if (Carrito.Any())
            {
                // Simulación de procesamiento de compra
                foreach (var articulo in Carrito)
                {
                    // Restar la cantidad vendida de la existencia
                    var articuloEnDB = _context.Articulos.FirstOrDefault(a => a.IdArticulo == articulo.IdArticulo);
                    if (articuloEnDB != null && articuloEnDB.Existencia >= 1)
                    {
                        articuloEnDB.Existencia -= 1;  // Aquí asumo que cada vez que se añade un artículo al carrito, se compra una unidad
                    }
                }

                Carrito.Clear();  // Limpiar el carrito después de la compra
                TempData["Message"] = "Compra realizada con éxito.";
            }
            else
            {
                TempData["ErrorMessage"] = "El carrito está vacío.";
            }

            // Redirigir a la página principal
            return RedirectToAction("Index");
        }

    }
}
