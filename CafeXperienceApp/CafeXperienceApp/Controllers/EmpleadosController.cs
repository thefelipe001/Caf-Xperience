using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeXperienceApp.Controllers
{
    [Authorize]

    public class EmpleadosController : Controller
    {
        private readonly IBaseRepository<Empleados> _Empleadorepositorio;
        private readonly IBaseRepository<TipoUsuario> _TipoUsuariorepositorio;
        private readonly ApplicationDbContext _context;


        public EmpleadosController(IBaseRepository<Empleados> Empleadorepositorio, IBaseRepository<TipoUsuario> TipoUsuariorepositorio, ApplicationDbContext context)
        {
            _Empleadorepositorio = Empleadorepositorio;
            _TipoUsuariorepositorio = TipoUsuariorepositorio;
            _context = context;
        }

        public ActionResult Index()
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
            var nombre = HttpContext.Request.Form["nombre"].FirstOrDefault();
            var cedula = HttpContext.Request.Form["cedula"].FirstOrDefault();
            var fechaingreso = HttpContext.Request.Form["fechaingreso"].FirstOrDefault();
            var estado = HttpContext.Request.Form["estado"].FirstOrDefault();
            var porcientocomision = HttpContext.Request.Form["porcientocomision"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            // Obtener los datos desde el repositorio
            var empleado = await _Empleadorepositorio.GetAll();
            List<EmpleadosViemModel> listado = empleado.Data.Select(item => new EmpleadosViemModel
            {
                IdUsuario = item.IdUsuario,
                IdEmpleado = item.IdEmpleado,
                Nombre = item.Nombre,
                Cedula = item.Cedula,
                Estado = item.Estado,
                TandaLabor = item.TandaLabor,
                PorcientoComision=item.PorcientoComision,
                FechaIngreso=item.FechaIngreso
            }).ToList();

            var totalRecords = empleado.Data.Count();
            var totalFilteredRecords = listado.Count;

            var data = listado.Skip(skip).Take(pageSize).ToList();

            return Json(new
            {
                draw = draw,
                recordsFiltered = totalFilteredRecords,
                recordsTotal = totalRecords,
                data = data
            });
        }


        [HttpPost]
        public async Task<JsonResult> GuardarAsync(Empleados empleado)
        {
            try
            {
                // Validar que los campos obligatorios no estén vacíos
                if (string.IsNullOrEmpty(empleado.Nombre) || string.IsNullOrEmpty(empleado.Cedula) || string.IsNullOrEmpty(empleado.TandaLabor))
                {
                    return Json(new { resultado = false, mensaje = "Todos los campos son obligatorios" });
                }

                // Establecer estado en función de la entrada
                empleado.Estado = empleado.Estado == "1" ? "A" : "I";

                Result<bool> resultado;
                if (empleado.IdUsuario == 0)
                {
                    // Si es un nuevo registro
                    resultado = await _Empleadorepositorio.Add(empleado);
                }
                else
                {
                    // Si es una actualización de un registro existente
                    resultado = await _Empleadorepositorio.Update(empleado);
                }

                if (resultado.Success)
                {
                    return Json(new { resultado = true });
                }
                else
                {
                    return Json(new { resultado = false, mensaje = resultado.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = "Error al guardar: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DataTipoUsuarios()
        {
            var data = await _TipoUsuariorepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }


        [HttpPost]
        public async Task<JsonResult> Eliminar([FromBody] Empleados empleados)
        {
            if (empleados.IdEmpleado <= 0)
            {
                return Json(new { resultado = false, mensaje = "ID inválido" });
            }

            var empleado = await _Empleadorepositorio.GetFirstAsync(u => u.IdEmpleado == empleados.IdEmpleado);

            if (empleado == null)
            {
                return Json(new { resultado = false, mensaje = "Empleado no encontrado" });
            }

            var result = await _Empleadorepositorio.Delete(empleado);

            if (result.Success)
            {
                return Json(new { resultado = true });
            }
            else
            {
                return Json(new { resultado = false, mensaje = result.ErrorMessage });
            }
        }

    }

}
