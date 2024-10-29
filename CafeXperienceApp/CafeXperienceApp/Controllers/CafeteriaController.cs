using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using CafeXperienceApp.Repositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeXperienceApp.Controllers
{
    [Authorize]

    public class CafeteriaController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IBaseRepository<Cafeteria> _repository;
        private readonly IBaseRepository<Usuario> _Usuariorepositorio;
        private readonly IBaseRepository<Campus> _Campusrepositorio;
        private readonly IBaseRepository<Empleados> _Empleadosrepositorio;


        public CafeteriaController(ILogger<CafeteriaController> logger, ApplicationDbContext db, IBaseRepository<Cafeteria> repository, IBaseRepository<Usuario> usuariorepositorio, IBaseRepository<Campus> campusrepositorio, IBaseRepository<Empleados> Empleadosrepositorio)
        {
            _db = db;
            _repository = repository;
            _Usuariorepositorio = usuariorepositorio;
            _Campusrepositorio = campusrepositorio;
            _Empleadosrepositorio = Empleadosrepositorio;
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
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            // Obtener los datos desde el repositorio
            var cafeteria = await _repository.GetAll();
            List<CafeteriaViewModel> listado = cafeteria.Data.Select(item => new CafeteriaViewModel
            {
                IdCafeteria = item.IdCafeteria,
                Descripcion = item.Descripcion,
                Encargado = _Usuariorepositorio.GetFirst(x => x.IdUsuario == item.IdEncargado)?.Data?.Nombre ?? "Desconocido",
                Campus = _Campusrepositorio.GetFirst(x => x.IdCampus == item.IdCampus)?.Data?.Descripcion ?? "Desconocido",
                Estado = item.Estado,
            }).ToList();

            // Filtrado
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower(); // Convertir el valor de búsqueda a minúsculas

                // Verificar si el valor de búsqueda es un número, para buscar por ID
                bool isNumericSearch = int.TryParse(searchValue, out int searchId);

                listado = listado.Where(u =>
                    (isNumericSearch && u.IdCafeteria == searchId) || // Buscar por ID si es numérico
                    u.Descripcion.ToLower().Contains(searchValue)  // Buscar por Nombre
                ).ToList();
            }

            var totalRecords = cafeteria.Data.Count(); // Método para contar registros totales
            var totalFilteredRecords = listado.Count; // Ya filtrado

            var data = listado.Skip(skip).Take(pageSize).ToList();

            return Json(new
            {
                draw = draw,
                recordsFiltered = totalFilteredRecords,
                recordsTotal = totalRecords,
                data = data
            });
        }



        [HttpGet]
        [Route("GetCafeteria")]
        public async Task<IActionResult> GetCafeteria([FromQuery] string id)
        {
            if (!string.IsNullOrEmpty(id))
                return Ok(await _repository.GetFirstAsync(q => q.IdCafeteria == int.Parse(id)));
            return Ok(await _repository.GetAll());   
        }




        [HttpPost]
        public async Task<IActionResult> DataCampus()
        {
            var data = await _Campusrepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }


        [HttpPost]
        public async Task<IActionResult> DataEncargado()
        {
            var data = await _Empleadosrepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }
        [HttpPost]
        public async Task<JsonResult> Eliminar([FromQuery] int IdCafeteria)
        {
            if (IdCafeteria <= 0)
            {
                return Json(new { resultado = false, mensaje = "ID inválido" });
            }

            var cafeteria = await _repository.GetFirstAsync(u => u.IdCafeteria == IdCafeteria);

            if (cafeteria == null)
            {
                return Json(new { resultado = false, mensaje = "Cafeteria no encontrado" });
            }

            var result = await _repository.Delete(cafeteria);

            if (result.Success)
            {
                return Json(new { resultado = true });
            }
            else
            {
                return Json(new { resultado = false, mensaje = result.ErrorMessage });
            }
        }







        [HttpPost]
        public async Task<JsonResult> GuardarAsync(Cafeteria cafeteria)
        {
            try
            {
                cafeteria.IdEncargado = 20;
                // Validar que los campos obligatorios no estén vacíos
                if (string.IsNullOrEmpty(cafeteria.Descripcion))
                {
                    return Json(new { resultado = false, mensaje = "Todos los campos son obligatorios" });
                }

                // Establecer estado en función de la entrada
                cafeteria.Estado = cafeteria.Estado == "1" ? "A" : "I";

                Result<bool> resultado;
                if (cafeteria.IdCafeteria == 0)
                {
                    // Si es un nuevo registro
                    resultado = await _repository.Add(cafeteria);
                }
                else
                {
                    // Si es una actualización de un registro existente
                    resultado = await _repository.Update(cafeteria);
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

    }
}



