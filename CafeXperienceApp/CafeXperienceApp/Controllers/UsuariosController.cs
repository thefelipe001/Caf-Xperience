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

            // Verificar el código de verificación
            var codigoVerificacion = await _context.CodigosVerificacion
                .Where(c => c.UsuarioId == usuario.IdUsuario && c.EsActivo)
                .FirstOrDefaultAsync();

            if (codigoVerificacion == null)
            {
                return BadRequest(new { message = "Código de verificación incorrecto o expirado" });
            }

            // Desactivar el código para que no se pueda usar nuevamente
            codigoVerificacion.EsActivo = true;
            _context.CodigosVerificacion.Update(codigoVerificacion);

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
        [ValidateAntiForgeryToken]
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

            // If the user does exist
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);  // Close Session

            return RedirectToAction("Login", "Usuarios");
        }

    }

}
