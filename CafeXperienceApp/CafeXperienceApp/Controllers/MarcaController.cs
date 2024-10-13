using Microsoft.AspNetCore.Mvc;
using CafeXperienceApp.Models;
using CafeXperienceApp.Interfaces;

namespace CafeXperienceApp.Controllers
{
    public class MarcaController : Controller
    {
        private readonly IBaseRepository<Marca> _repository;

        public MarcaController(IBaseRepository<Marca> repository)
        {
            _repository = repository;
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
            var result = await _repository.GetAll();

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

                query = query.Where(m =>
                    (isNumericSearch && m.IdMarca == searchId) ||    // Buscar por ID si es numérico
                    m.Descripcion.ToLower().Contains(searchValue) ||        // Buscar por Descripción
                    m.Estado.ToLower().Contains(searchValue)                // Buscar por Estado
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
        public async Task<JsonResult> GuardarAsync(Marca marca, IFormFile Imagen)
        {
            try
            {
                // Manejo de la imagen
                if (Imagen != null && Imagen.Length > 0)
                {
                    var fileName = Path.GetFileName(Imagen.FileName); // Obtener el nombre del archivo

                    // Definir la ruta donde se guardará la imagen
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/design/images", fileName);

                    // Si el archivo existe, reemplazarlo
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path); // Eliminar el archivo existente
                    }

                    // Guardar la nueva imagen
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await Imagen.CopyToAsync(stream); // Copiar el archivo de imagen de manera asincrónica
                    }

                    // Guardar la ruta de la imagen en la base de datos
                    marca.RutaImagen = $"~/design/images/{fileName}";
                }

                // Asignar el estado correctamente
                marca.Estado = marca.Estado == "1" ? "A" : "I";

                Result<bool> result;

                if (marca.IdMarca == 0)
                {
                    // Si es un nuevo registro
                    result = await _repository.Add(marca);
                }
                else
                {
                    // Si es una actualización de un registro existente
                    result = await _repository.Update(marca);
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
        public async Task<JsonResult> Eliminar([FromBody] Marca marca)
        {
            try
            {
                if (marca.IdMarca > 0)
                {
                    var result = await _repository.Delete(marca);

                    // Verificar si la eliminación fue exitosa
                    if (result.Success)
                    {
                        return Json(new { resultado = true });
                    }
                    else
                    {
                        if (result.ErrorMessage=="Error al Eliminar: An error occurred while saving the entity changes. See the inner exception for details.")
                        {
                            result.ErrorMessage = "Debe reasignar los Marca o eliminarlos antes de eliminar esta marca.";
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
