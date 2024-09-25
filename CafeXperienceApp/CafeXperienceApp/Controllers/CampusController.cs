using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeXperienceApp.Models;
using CafeXperienceApp.Interfaces;

namespace CafeXperienceApp.Controllers
{
    public class CampusController : Controller
    {
        private readonly IBaseRepository<Campus> _campusrepositorio;

        public CampusController(IBaseRepository<Campus> campusrepositorio)
        {
            _campusrepositorio = campusrepositorio;
        }

        // GET: Campus
        public  IActionResult Index()
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
            var result = await _campusrepositorio.GetAll();

            // Verificar si la operaci�n fue exitosa
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

            // Aplicar el filtro de b�squeda si es necesario
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();

                // Verificar si el valor de b�squeda es un n�mero, para buscar por ID
                bool isNumericSearch = int.TryParse(searchValue, out int searchId);

                query = query.Where(u =>
                    (isNumericSearch && u.IdCampus == searchId) ||    // Buscar por ID si es num�rico
                    u.Descripcion.ToLower().Contains(searchValue) ||        // Buscar por Descripci�n
                    u.Estado.ToLower().Contains(searchValue)                // Buscar por Estado
                );
            }

            // Mejorar el conteo: calcular el total de registros filtrados despu�s del filtro
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
        public async Task<JsonResult> GuardarAsync(Campus campus)
        {
            try
            {
                // Asignar el estado correctamente
                campus.Estado = campus.Estado == "1" ? "A" : "I";

                Result<bool> result;

                if (campus.IdCampus == 0)
                {
                    // Si es un nuevo registro
                    result = await _campusrepositorio.Add(campus);
                }
                else
                {
                    // Si es una actualizaci�n de un registro existente
                    result = await _campusrepositorio.Update(campus);
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
        public async Task<JsonResult> Eliminar([FromBody] Campus campus)
        {
            try
            {
                if (campus.IdCampus > 0)
                {
                    var result = await _campusrepositorio.Delete(campus);

                    // Verificar si la eliminaci�n fue exitosa
                    if (result.Success)
                    {
                        return Json(new { resultado = true });
                    }
                    else
                    {
                        if (result.ErrorMessage == "Error al Eliminar: An error occurred while saving the entity changes. See the inner exception for details.")
                        {
                            result.ErrorMessage = "Debe reasignar los Campus o eliminarlos antes de eliminar la cafeteria.";
                        }
                        // Asignar el estado correctamente
                        return Json(new { resultado = false, mensaje = result.ErrorMessage });
                    }
                }
                return Json(new { resultado = false, mensaje = "ID inv�lido" });
            }
            catch (Exception ex)
            {

                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }

    }
}
