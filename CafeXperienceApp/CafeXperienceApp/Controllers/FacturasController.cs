using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeXperienceApp.Controllers
{
    [Authorize]

    public class FacturasController : Controller
    {
        private readonly IBaseRepository<FacturacionDetalleReporte> _facturacionDetalleReporteRepositorio;
        private readonly IBaseRepository<Usuario> _usuarioRepositorio;
        private readonly IBaseRepository<Campus> _campusRepositorio;
        private readonly IBaseRepository<Proveedore> _proveedoreRepositorio;

        public FacturasController(IBaseRepository<FacturacionDetalleReporte> facturacionDetalleReporteRepositorio, IBaseRepository<Campus> campusRepositorio, IBaseRepository<Proveedore> proveedoreRepositorio, IBaseRepository<Usuario> usuarioRepositorio)
        {
            _facturacionDetalleReporteRepositorio = facturacionDetalleReporteRepositorio;
            _campusRepositorio = campusRepositorio;
            _proveedoreRepositorio = proveedoreRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

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

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Data()
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();

            // Parámetros de búsqueda personalizados
            var usuario = HttpContext.Request.Form["Usuario"].FirstOrDefault();
            var campus = HttpContext.Request.Form["Campus"].FirstOrDefault();
            var fecha = HttpContext.Request.Form["Fecha"].FirstOrDefault();
            var proveedor = HttpContext.Request.Form["Proveedor"].FirstOrDefault();
            var monto = HttpContext.Request.Form["Monto"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            // Obtener los datos desde el repositorio
            var resultado = await _facturacionDetalleReporteRepositorio.GetAll();
            var reporte = resultado.Data; // Extrae la propiedad Data para obtener el IEnumerable<FacturacionDetalleReporte>

            // Filtrar por Nombre de Usuario
            if (!string.IsNullOrEmpty(usuario))
            {
                reporte = reporte.Where(r => r.NombreUsuario.ToLower().Contains(usuario.ToLower())).ToList();
            }

            // Filtrar por DescripcionCampus
            if (!string.IsNullOrEmpty(campus))
            {
                reporte = reporte.Where(r => r.DescripcionCampus.ToLower().Contains(campus.ToLower())).ToList();
            }

            // Filtrar por FechaVenta
            if (!string.IsNullOrEmpty(fecha))
            {
                DateTime fechaParsed;
                if (DateTime.TryParse(fecha, out fechaParsed))
                {
                    reporte = reporte.Where(r => r.FechaVenta.Date == fechaParsed.Date).ToList();
                }
            }

            // Filtrar por NombreComercial (Proveedor)
            if (!string.IsNullOrEmpty(proveedor))
            {
                reporte = reporte.Where(r => r.NombreComercial.ToLower().Contains(proveedor.ToLower())).ToList();
            }

            // Filtrar por Monto (MontoArticulo)
            if (!string.IsNullOrEmpty(monto))
            {
                decimal montoParsed;
                if (decimal.TryParse(monto, out montoParsed))
                {
                    reporte = reporte.Where(r => r.Monto == montoParsed).ToList();
                }
            }

            // Contar el total de registros antes y después de los filtros
            var totalRecords = resultado.Data.Count(); // Usa el total original
            var totalFilteredRecords = reporte.Count(); // Usa el total filtrado

            // Aplicar paginación
            var data = reporte.Skip(skip).Take(pageSize).ToList();

            return Json(new
            {
                draw = draw,
                recordsFiltered = totalFilteredRecords,
                recordsTotal = totalRecords,
                data = data
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> DataUsuario()
        {
            var data = await _usuarioRepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }
        
        [HttpPost]
        public async Task<IActionResult> DataCampus()
        {
            var data = await _campusRepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }

        [HttpPost]
        public async Task<IActionResult> DataProveedor()
        {
            var data = await _proveedoreRepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }
    }
}
