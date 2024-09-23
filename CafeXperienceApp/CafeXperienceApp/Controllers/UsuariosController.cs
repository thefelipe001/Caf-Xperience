using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeXperienceApp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IBaseRepository<Usuario> _Usuariorepositorio;
        private readonly IBaseRepository<TipoUsuario> _TipoUsuariorepositorio;

        public UsuariosController(IBaseRepository<Usuario> Usuariorepositorio, IBaseRepository<TipoUsuario> TipoUsuariorepositorio)
        {
            _Usuariorepositorio = Usuariorepositorio;
            _TipoUsuariorepositorio = TipoUsuariorepositorio;
        }

        public ActionResult Index()
        {
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
            var usuarios = await _Usuariorepositorio.GetAll();
            List<UsuariosViewModel> listado = usuarios.Data.Select(item => new UsuariosViewModel
            {
                IdUsuario = item.IdUsuario,
                Nombre = item.Nombre,
                Cedula = item.Cedula,
                FechaRegistro = item.FechaRegistro,
                LimiteCredito = item.LimiteCredito,
                Correo = item.Correo,
                Estado = item.Estado,
                TipoUsuario = _TipoUsuariorepositorio.GetFirst(x => x.IdTipoUsuarios == item.TipoUsuarioId)?.Data?.Descripcion ?? "Desconocido"
            }).ToList();

            // Filtrado
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower(); // Convertir el valor de búsqueda a minúsculas

                // Verificar si el valor de búsqueda es un número, para buscar por ID
                bool isNumericSearch = int.TryParse(searchValue, out int searchId);

                listado = listado.Where(u =>
                    (isNumericSearch && u.IdUsuario == searchId) || // Buscar por ID si es numérico
                    u.Nombre.ToLower().Contains(searchValue) || // Buscar por Nombre
                    u.Cedula.ToLower().Contains(searchValue) // Buscar por Cedula
                ).ToList();
            }

            var totalRecords = usuarios.Data.Count(); // Método para contar registros totales
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

        [HttpPost]
        public async Task<JsonResult> GuardarAsync(Usuario usuario)
        {
            try
            {
                // Validar que los campos obligatorios no estén vacíos
                if (string.IsNullOrEmpty(usuario.Nombre) || string.IsNullOrEmpty(usuario.Cedula) || string.IsNullOrEmpty(usuario.Correo))
                {
                    return Json(new { resultado = false, mensaje = "Todos los campos son obligatorios" });
                }

                // Establecer estado en función de la entrada
                usuario.Estado = usuario.Estado == "1" ? "A" : "I";

                Result<bool> resultado;
                if (usuario.IdUsuario == 0)
                {
                    // Si es un nuevo registro
                    resultado = await _Usuariorepositorio.Add(usuario);
                }
                else
                {
                    // Si es una actualización de un registro existente
                    resultado = await _Usuariorepositorio.Update(usuario);
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
        public async Task<JsonResult> Eliminar([FromQuery] int IdUsuario)
        {
            if (IdUsuario <= 0)
            {
                return Json(new { resultado = false, mensaje = "ID inválido" });
            }

            var usuario = await _Usuariorepositorio.GetFirstAsync(u => u.IdUsuario == IdUsuario);

            if (usuario == null)
            {
                return Json(new { resultado = false, mensaje = "Usuario no encontrado" });
            }

            var result = await _Usuariorepositorio.Delete(usuario);

            if (result.Success)
            {
                return Json(new { resultado = true });
            }
            else
            {
                return Json(new { resultado = false, mensaje = result.ErrorMessage });
            }
        }


        public ActionResult Login()
        {
            return View();
        }
    }

}
