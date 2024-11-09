using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Security.Claims;

namespace CafeXperienceApp.Controllers
{
    [Authorize]

    public class ReporteRentaController : Controller
    {

        private readonly IBaseRepository<FacturacionDetalleReporte> _FacturacionDetalleReporterepositorio;
        private readonly IBaseRepository<Campus> _Campusrepositorio;
        private readonly IBaseRepository<Proveedore> _Proveedorerepositorio;
        private readonly IWebHostEnvironment _host;


        public ReporteRentaController(IBaseRepository<FacturacionDetalleReporte> FacturacionDetalleReporterepositorio, IWebHostEnvironment host, IBaseRepository<Campus> Campusrepositorio, IBaseRepository<Proveedore> Proveedorerepositorio)
        {
            _FacturacionDetalleReporterepositorio = FacturacionDetalleReporterepositorio;
            _host = host;
            _Campusrepositorio = Campusrepositorio;
            _Proveedorerepositorio = Proveedorerepositorio;
        }



        public IActionResult Index()
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
            var campus = HttpContext.Request.Form["campus"].FirstOrDefault();
            var fecha = HttpContext.Request.Form["Fecha"].FirstOrDefault();
            var proveedor = HttpContext.Request.Form["Proveedor"].FirstOrDefault();
            var monto = HttpContext.Request.Form["Monto"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            // Obtener los datos desde el repositorio
            var resultado = await _FacturacionDetalleReporterepositorio.GetAll();
            var reporte = resultado.Data; // Extrae la propiedad Data para obtener el IEnumerable<FacturacionDetalleReporte>

            // Filtrar por DescripcionCampus
            if (!string.IsNullOrEmpty(campus))
            {
                reporte = reporte.Where(r => r.DescripcionCampus.ToLower().Contains(campus.ToLower())).ToList();
            }

            // Filtrar por FechaVenta
            if (!string.IsNullOrEmpty(fecha))
            {
                DateTime fechaParsed;
                if (DateTime.TryParse(fecha, out fechaParsed))
                {
                    reporte = reporte.Where(r => r.FechaVenta.Date == fechaParsed.Date).ToList();
                }
            }

            // Filtrar por NombreComercial (Proveedor)
            if (!string.IsNullOrEmpty(proveedor))
            {
                reporte = reporte.Where(r => r.NombreComercial.ToLower().Contains(proveedor.ToLower())).ToList();
            }

            // Filtrar por Monto (MontoArticulo)
            if (!string.IsNullOrEmpty(monto))
            {
                decimal montoParsed;
                if (decimal.TryParse(monto, out montoParsed))
                {
                    reporte = reporte.Where(r => r.Monto == montoParsed).ToList();
                }
            }

            // Contar el total de registros antes y después de los filtros
            var totalRecords = resultado.Data.Count(); // Usa el total original
            var totalFilteredRecords = reporte.Count(); // Usa el total filtrado

            // Aplicar paginación
            var data = reporte.Skip(skip).Take(pageSize).ToList();

            return Json(new
            {
                draw = draw,
                recordsFiltered = totalFilteredRecords,
                recordsTotal = totalRecords,
                data = data
            });
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
        public async Task<IActionResult> DataProvedor()
        {
            var data = await _Proveedorerepositorio.GetAll();

            // Verificar si hay un error en la obtención de los datos
            if (!data.Success)
            {
                return Json(new { error = data.ErrorMessage });
            }

            return Json(new { data = data.Data });
        }



        public async Task<IActionResult> DescargarPDF()
        {
            // Configurar la licencia de QuestPDF
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Obtener los datos de la vista vw_FacturacionDetallesReporte
            var facturacionDetalles = await _FacturacionDetalleReporterepositorio.GetAll();

            // Calcular el total de dinero generado
            var totalDineroGenerado = facturacionDetalles.Data.Sum(item => item.Monto * item.Cantidad);

            // Generar el documento PDF
            var data = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.Size(PageSizes.A4);

                    // Encabezado del PDF
                    page.Header().Row(row =>
                    {
                        // Ruta de la imagen del logo
                        var rutaImagen = Path.Combine(_host.WebRootPath, "images/VisualStudio.png");
                        byte[] imageData = System.IO.File.Exists(rutaImagen) ? System.IO.File.ReadAllBytes(rutaImagen) : null;

                        if (imageData != null)
                            row.ConstantItem(100).Image(imageData);

                        // Información de la empresa en el centro
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignCenter().Text("Caferia UNAPEC").Bold().FontSize(20);
                            col.Item().AlignCenter().Text("Maximo Gomez #27 ").FontSize(10);
                            col.Item().AlignCenter().Text("829-901-2122").FontSize(10);
                            col.Item().AlignCenter().Text("Cafex@unapec.edu.do").FontSize(10);
                        });

                        // Información adicional a la derecha
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Border(1).BorderColor("#257272")
                                .AlignCenter().Text("RUC 123456789").Bold();

                            col.Item().Background("#257272").Border(1)
                                .BorderColor("#257272").AlignCenter()
                                .Text("Reporte de Facturación").FontColor("#fff").FontSize(12).Bold();

                            col.Item().Border(1).BorderColor("#257272")
                                .AlignCenter().Text(DateTime.Now.ToString("yyyy-MM-dd")).FontSize(10);
                        });
                    });

                    // Contenido Principal del PDF
                    page.Content().PaddingVertical(20).Column(col1 =>
                    {
                        col1.Spacing(5);
                        col1.Item().Text("Detalles de Facturación").Underline().Bold().FontSize(16);

                        // Tabla con los datos de facturación
                        col1.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // NumFactura
                                columns.RelativeColumn(2); // FechaVenta
                                columns.RelativeColumn(1); // Monto
                                columns.RelativeColumn(1); // Cantidad
                                columns.RelativeColumn(2); // NombreUsuario
                                columns.RelativeColumn(2); // DescripcionArticulo
                            });

                            // Encabezado de la tabla
                            table.Header(header =>
                            {
                                header.Cell().Background("#257272").Padding(5).Text("NumFactura").FontColor("#fff").Bold();
                                header.Cell().Background("#257272").Padding(5).Text("FechaVenta").FontColor("#fff").Bold();
                                header.Cell().Background("#257272").Padding(5).Text("Monto").FontColor("#fff").Bold();
                                header.Cell().Background("#257272").Padding(5).Text("Cantidad").FontColor("#fff").Bold();
                                header.Cell().Background("#257272").Padding(5).Text("Usuario").FontColor("#fff").Bold();
                                header.Cell().Background("#257272").Padding(5).Text("Artículo").FontColor("#fff").Bold();
                            });

                            // Filas de la tabla con datos de facturación
                            foreach (var item in facturacionDetalles.Data)
                            {
                                table.Cell().Padding(5).Text(item.NumFactura.ToString());
                                table.Cell().Padding(5).Text(item.FechaVenta.ToString("dd/MM/yyyy"));
                                table.Cell().Padding(5).Text(item.Monto.ToString("C"));
                                table.Cell().Padding(5).Text(item.Cantidad.ToString());
                                table.Cell().Padding(5).Text(item.NombreUsuario);
                                table.Cell().Padding(5).Text(item.DescripcionArticulo);
                            }
                        });

                        col1.Spacing(20);

                        // Mostrar el total de dinero generado
                        col1.Item().Padding(5).Text($"Total de dinero generado: {totalDineroGenerado.ToString("C")}").Bold().FontSize(14);

                        // Sección de comentarios adicionales
                        col1.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
                        {
                            column.Item().Text("Comentarios").FontSize(14).Bold();
                            column.Item().Text("Aquí puedes añadir cualquier comentario adicional o nota relevante sobre el reporte de facturación.");
                        });
                    });

                    // Pie de página con número de página
                    page.Footer().AlignRight().Text(txt =>
                    {
                        txt.Span("Página ").FontSize(10);
                        txt.CurrentPageNumber().FontSize(10);
                        txt.Span(" de ").FontSize(10);
                        txt.TotalPages().FontSize(10);
                    });
                });
            }).GeneratePdf();

            // Retornar el PDF como archivo descargable
            Stream stream = new MemoryStream(data);
            return File(stream, "application/pdf", "reporte_facturacion.pdf");
        }

    }
}
