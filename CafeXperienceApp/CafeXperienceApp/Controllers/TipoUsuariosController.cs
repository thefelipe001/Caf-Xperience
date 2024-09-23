using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeXperienceApp.Data;
using CafeXperienceApp.Models;
using CafeXperienceApp.Repositorio;
using CafeXperienceApp.Interfaces;

namespace CafeXperienceApp.Controllers
{
    public class TipoUsuariosController : Controller
    {
        private readonly IBaseRepository<TipoUsuario> _tipoUsuariorepositorio;

        public TipoUsuariosController(IBaseRepository<TipoUsuario> tipoUsuariorepositorio)
        {
            _tipoUsuariorepositorio = tipoUsuariorepositorio;
        }

        public IActionResult Index()
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
            var result = await _tipoUsuariorepositorio.GetAll();

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
                    (isNumericSearch && u.IdTipoUsuarios == searchId) ||    // Buscar por ID si es numérico
                    u.Descripcion.ToLower().Contains(searchValue) ||        // Buscar por Descripción
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
        public async Task<JsonResult> GuardarAsync(TipoUsuario tipoUsuario)
        {
            try
            {
                // Asignar el estado correctamente
                tipoUsuario.Estado = tipoUsuario.Estado == "1" ? "A" : "I";

                Result<bool> result;

                if (tipoUsuario.IdTipoUsuarios == 0)
                {
                    // Si es un nuevo registro
                    result = await _tipoUsuariorepositorio.Add(tipoUsuario);
                }
                else
                {
                    // Si es una actualización de un registro existente
                    result = await _tipoUsuariorepositorio.Update(tipoUsuario);
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
        public async Task<JsonResult> Eliminar([FromBody] TipoUsuario tipoUsuario)
        {
            try
            {
                if (tipoUsuario.IdTipoUsuarios > 0)
                {
                    var result = await _tipoUsuariorepositorio.Delete(tipoUsuario);

                    // Verificar si la eliminación fue exitosa
                    if (result.Success)
                    {
                        return Json(new { resultado = true });
                    }
                    else
                    {
                        if (result.ErrorMessage=="Error al Eliminar: An error occurred while saving the entity changes. See the inner exception for details.")
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
