using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CafeXperienceApp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IBaseRepository<Usuario> _Usuariorepositorio;
        private readonly IBaseRepository<TipoUsuario> _TipoUsuariorepositorio;
        private readonly ApplicationDbContext _context;


        public UsuariosController(IBaseRepository<Usuario> Usuariorepositorio, IBaseRepository<TipoUsuario> TipoUsuariorepositorio, ApplicationDbContext context)
        {
            _Usuariorepositorio = Usuariorepositorio;
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
            var fechaRegistro = HttpContext.Request.Form["fechaRegistro"].FirstOrDefault();
            var estado = HttpContext.Request.Form["estado"].FirstOrDefault();
            var limiteCredito = HttpContext.Request.Form["limiteCredito"].FirstOrDefault();

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
            if (!string.IsNullOrEmpty(fechaRegistro))
            {
                DateTime fecha = DateTime.Parse(fechaRegistro);
                listado = listado.Where(u => u.FechaRegistro.Date == fecha.Date).ToList();
            }

            // Filtrar por estado
            if (!string.IsNullOrEmpty(estado))
            {
                listado = listado.Where(u => u.Estado == estado).ToList();
            }

            // Filtrar por límite de crédito
            if (!string.IsNullOrEmpty(limiteCredito))
            {
                decimal credito = decimal.Parse(limiteCredito);
                listado = listado.Where(u => u.LimiteCredito == credito).ToList();
            }

            var totalRecords = usuarios.Data.Count();
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
        public async Task<JsonResult> RegistrarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                // Validar que los campos obligatorios no estén vacíos
                if (string.IsNullOrEmpty(usuario.Nombre) || string.IsNullOrEmpty(usuario.Cedula) || string.IsNullOrEmpty(usuario.Correo))
                {
                    return Json(new { resultado = false, mensaje = "Todos los campos son obligatorios" });
                }

                // Establecer estado en función de la entrada
                usuario.Estado ="A";
                usuario.TipoUsuarioId = 25;

                Result<bool> resultado;

                resultado = await _Usuariorepositorio.Add(usuario);
            

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

            string Rol=string.Empty;
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
