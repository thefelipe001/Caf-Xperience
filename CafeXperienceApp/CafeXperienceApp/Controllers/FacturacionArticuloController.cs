using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeXperienceApp.Controllers
{
    [Authorize]

    public class FacturacionArticuloController : Controller
    {
        private readonly IBaseRepository<FacturacionArticulo> _facturacionArticulorepositorio;

        private readonly IBaseRepository<Articulo> _articulorepositorio;

        private readonly IBaseRepository<Empleados> _empleadosrepositorio;

        private readonly IBaseRepository<Usuario> _usuariorepositorio;

        private readonly IBaseRepository<Campus> _campusrepositorio;



        public FacturacionArticuloController(IBaseRepository<Articulo> articulorepositorio, IBaseRepository<FacturacionArticulo> facturacionArticulorepositorio, IBaseRepository<Empleados> empleadosrepositorio, IBaseRepository<Usuario> usuariorepositorio, IBaseRepository<Campus> campusrepositorio)
        {
            _articulorepositorio = articulorepositorio;
            _facturacionArticulorepositorio = facturacionArticulorepositorio;
            _usuariorepositorio = usuariorepositorio;
            _empleadosrepositorio = empleadosrepositorio;
            _campusrepositorio = campusrepositorio;
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
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            // Obtener los datos desde el repositorio
            var result = await _facturacionArticulorepositorio.GetAll();

            // Verificar si la operación fue exitosa
            if (!result.Success)
            {
                // En caso de error, devolver el mensaje de error a DataTables
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    error = result.ErrorMessage
                });
            }

            var query = result.Data;

            // Aplicar el filtro de búsqueda si es necesario
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();

                // Verificar si el valor de búsqueda es un número, para buscar por ID
                bool isNumericSearch = int.TryParse(searchValue, out int searchId);

                query = query.Where(u =>
                    (isNumericSearch && u.NoFactura == searchId) ||    // Buscar por ID si es numérico
                    u.Estado.ToLower().Contains(searchValue)                // Buscar por Estado
                );
            }

            // Mejorar el conteo: calcular el total de registros filtrados después del filtro
            var totalRecords = query.Count();
            var data = query.Skip(skip).Take(pageSize).ToList(); // Obtener los datos paginados

            return Json(new
            {
                draw = draw,
                recordsFiltered = totalRecords,  // Total de registros filtrados
                recordsTotal = totalRecords,     // Total de registros
                data = data
            });
        }


        [HttpPost]
        public async Task<JsonResult> GuardarAsync(FacturacionArticulo facturacionArticulo)
        {
            try
            {
                facturacionArticulo.FechaVenta = DateTime.Now;
                facturacionArticulo.Estado = facturacionArticulo.Estado == "1" ? "A" : "I";

                Result<bool> result;

                if (facturacionArticulo.NoFactura == 0)
                {
                    // Si es un nuevo registro
                    result = await _facturacionArticulorepositorio.Add(facturacionArticulo);
                }
                else
                {
                    // Si es una actualización de un registro existente
                    result = await _facturacionArticulorepositorio.Update(facturacionArticulo);
                }

                // Verificar el resultado
                if (result.Success)
                {
                    return Json(new { resultado = true });
                }
                else
                {
                    return Json(new { resultado = false, mensaje = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DataArticulo()
        {
            var data = await _articulorepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }


        [HttpPost]
        public async Task<IActionResult> DataEmpleado()
        {
            var data = await _empleadosrepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }

        [HttpPost]
        public async Task<IActionResult> DataUsuario()
        {
            var data = await _usuariorepositorio.GetAll();

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
            var data = await _campusrepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }


        [HttpPost]
        public async Task<JsonResult> Eliminar([FromBody] FacturacionArticulo facturacionArticulo)
        {
            try
            {
                if (facturacionArticulo.NoFactura > 0)
                {
                    var result = await _facturacionArticulorepositorio.Delete(facturacionArticulo);

                    // Verificar si la eliminación fue exitosa
                    if (result.Success)
                    {
                        return Json(new { resultado = true });
                    }
                    else
                    {
                        if (result.ErrorMessage == "Error al Eliminar: An error occurred while saving the entity changes. See the inner exception for details.")
                        {
                            result.ErrorMessage = "Debe reasignar los usuarios o eliminarlos antes de eliminar este tipo de usuario.";
                        }
                        // Asignar el estado correctamente
                        return Json(new { resultado = false, mensaje = result.ErrorMessage });
                    }
                }
                return Json(new { resultado = false, mensaje = "ID inválido" });
            }
            catch (Exception ex)
            {

                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }
    }
}
