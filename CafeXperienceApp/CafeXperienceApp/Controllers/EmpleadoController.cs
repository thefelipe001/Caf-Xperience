using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CafeXperienceApp.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly IBaseRepository<Empleados> _Empleadorepositorio;
        private readonly IBaseRepository<TipoUsuario> _TipoUsuariorepositorio;
        private readonly ApplicationDbContext _context;


        public EmpleadoController(IBaseRepository<Empleados> Empleadorepositorio, IBaseRepository<TipoUsuario> TipoUsuariorepositorio, ApplicationDbContext context)
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
                Nombre = item.Nombre,
                Cedula = item.Cedula,
                Estado = item.Estado,
            }).ToList();

            // Filtrar por nombre
            if (!string.IsNullOrEmpty(nombre))
            {
                listado = listado.Where(u => u.Nombre.ToLower().Contains(nombre.ToLower())).ToList();
            }

            // Filtrar por cédula
            if (!string.IsNullOrEmpty(cedula))
            {
                listado = listado.Where(u => u.Cedula.Contains(cedula)).ToList();
            }

            // Filtrar por fecha de registro
            if (!string.IsNullOrEmpty(fechaingreso))
            {
                DateTime fecha = DateTime.Parse(fechaingreso);
                listado = listado.Where(u => u.FechaIngreso.Date == fecha.Date).ToList();
            }

            // Filtrar por estado
            if (!string.IsNullOrEmpty(estado))
            {
                listado = listado.Where(u => u.Estado == estado).ToList();
            }

            // Filtrar por límite de crédito
            if (!string.IsNullOrEmpty(porcientocomision))
            {
                decimal credito = decimal.Parse(porcientocomision);
                listado = listado.Where(u => u.PorcientoComision == credito).ToList();
            }

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
        public async Task<JsonResult> Eliminar([FromQuery] int IdUsuario)
        {
            if (IdUsuario <= 0)
            {
                return Json(new { resultado = false, mensaje = "ID inválido" });
            }

            var empleado = await _Empleadorepositorio.GetFirstAsync(u => u.IdUsuario == IdUsuario);

            if (empleado == null)
            {
                return Json(new { resultado = false, mensaje = "Usuario no encontrado" });
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


        public ActionResult Login()
        {
            return View();
        }



        public IActionResult RegistrarCliente()
        {
            return View();
        }

        public IActionResult CambioClave()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> cambiarcontrasena([FromBody] CambioContrasenaDto cambioContrasenaDto)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos inválidos" });
            }

            // Verificar que el usuario exista con el correo proporcionado
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == cambioContrasenaDto.Email);
            if (usuario == null)
            {
                return BadRequest(new { message = "Usuario no encontrado" });
            }


            // Encriptar y cambiar la contraseña
            _context.Usuarios.Update(usuario);

            // Guardar los cambios en la base de datos
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Contraseña cambiada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al cambiar la contraseña", error = ex.Message });
            }
        }


        [HttpPost("api/registro")]
        public async Task<JsonResult> RegistrarUsuario([FromBody] Empleados empleado)
        {
            try
            {
                // Validar que los campos obligatorios no estén vacíos
                if (string.IsNullOrEmpty(empleado.Nombre) || string.IsNullOrEmpty(empleado.Cedula) || string.IsNullOrEmpty(empleado.TandaLabor))
                {
                    return Json(new { resultado = false, mensaje = "Todos los campos son obligatorios" });
                }

                // Establecer estado en función de la entrada
                empleado.Estado = "A";
                //empleado.TipoUsuarioId != 25;

                Result<bool> resultado;

                resultado = await _Empleadorepositorio.Add(empleado);


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
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Buscar al usuario por su correo y contraseña
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == loginDto.Email && u.Contraseña == loginDto.Password);

            // Validar si el usuario existe y si la contraseña es correcta
            if (usuario == null)
            {
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            string Rol = string.Empty;
            if (usuario.TipoUsuarioId == 25)
                Rol = "Cliente";

            else
                Rol = "Empleado";





            // If the user does exist
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim("LimiteCredito", usuario.LimiteCredito.ToString("C")),
                new Claim("Rol", Rol)
            };



            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); // Register structure
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };   // Create properties

            await HttpContext.SignInAsync(  // Register user login
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    properties
                );



            return RedirectToAction("Index", "Dashboard");


        }


        public async Task<IActionResult> Singout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Usuarios");
        }

    }

}
