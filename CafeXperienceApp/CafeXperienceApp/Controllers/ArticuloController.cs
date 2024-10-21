using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CafeXperienceApp.Controllers
{
    public class ArticuloController : Controller
    {
        private readonly IBaseRepository<Articulo> _articulorepositorio;
        private readonly IBaseRepository<Marca> _Marcarepositorio;
        private readonly IBaseRepository<Proveedore> _Proveedoresrepositorio;

        public ArticuloController(IBaseRepository<Articulo> articulorepositorio, IBaseRepository<Marca> marcarepositorio, IBaseRepository<Proveedore> proveedoresrepositorio)
        {
            _articulorepositorio = articulorepositorio;
            _Marcarepositorio = marcarepositorio;
            _Proveedoresrepositorio = proveedoresrepositorio;
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
            var result = await _articulorepositorio.GetAll();

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
                    (isNumericSearch && u.IdArticulo == searchId) ||    // Buscar por ID si es numérico
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
        public async Task<JsonResult> GuardarAsync(Articulo articulo, IFormFile Imagen)
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
                    articulo.RutaImagen = $"~/design/images/{fileName}";
                }
                // Asignar el estado correctamente
                articulo.Estado = articulo.Estado == "1" ? "A" : "I";

                Result<bool> result;

                if (articulo.IdArticulo == 0)
                {
                    // Si es un nuevo registro
                    result = await _articulorepositorio.Add(articulo);
                }
                else
                {
                    // Si es una actualización de un registro existente
                    result = await _articulorepositorio.Update(articulo);
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
        public async Task<IActionResult> DataMarca()
        {
            var data = await _Marcarepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }


        [HttpPost]
        public async Task<IActionResult> DataProveedores()
        {
            var data = await _Proveedoresrepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }

        [HttpPost]
        public async Task<JsonResult> Eliminar([FromBody] Articulo articulo)
        {
            try
            {
                if (articulo.IdArticulo > 0)
                {
                    var result = await _articulorepositorio.Delete(articulo);

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
